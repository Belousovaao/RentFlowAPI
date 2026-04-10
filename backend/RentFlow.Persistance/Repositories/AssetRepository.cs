using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Persistance;

namespace RentFlow.Infrastructure.Persistence.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly RentFlowDbContext _context;

    public AssetRepository(RentFlowDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Asset>> SearchAsync(AssetFilter filter, CancellationToken ct = default)
    {
        var query = _context.Assets
            .Include(a => a.Photos)
            .Include(a => a.Bookings) // Включаем бронирования для проверки доступности
            .AsQueryable();

        // Поиск по тексту
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(a => 
                a.BrandName.Contains(filter.Search) ||
                a.Model.Contains(filter.Search) ||
                a.Code.Contains(filter.Search));
        }

        // Фильтр по бренду
        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            query = query.Where(a => a.BrandName == filter.Brand);
        }

        // Фильтр по типу
        if (filter.Type.HasValue)
        {
            query = query.Where(a => a.Type == filter.Type.Value);
        }

        // Фильтр по категории
        if (filter.Category.HasValue)
        {
            query = query.Where(a => a.Category == filter.Category.Value);
        }

        // Фильтр по году
        if (filter.YearFrom.HasValue)
        {
            query = query.Where(a => a.Year >= filter.YearFrom.Value);
        }
        if (filter.YearTo.HasValue)
        {
            query = query.Where(a => a.Year <= filter.YearTo.Value);
        }

        // Фильтр по типу топлива
        if (filter.FuelType.HasValue)
        {
            query = query.Where(a => a.FuelType == filter.FuelType.Value);
        }

        // Фильтр по трансмиссии
        if (filter.Transmission.HasValue)
        {
            query = query.Where(a => a.Transmission == filter.Transmission.Value);
        }

        // Фильтр по типу привода
        if (filter.DriveType.HasValue)
        {
            query = query.Where(a => a.DriveType == filter.DriveType.Value);
        }

        // Фильтр по количеству мест
        if (filter.MinSeats.HasValue)
        {
            query = query.Where(a => a.Seats >= filter.MinSeats.Value);
        }
        if (filter.MaxSeats.HasValue)
        {
            query = query.Where(a => a.Seats <= filter.MaxSeats.Value);
        }

        // Фильтр по количеству дверей
        if (filter.MinDoors.HasValue)
        {
            query = query.Where(a => a.Doors >= filter.MinDoors.Value);
        }
        if (filter.MaxDoors.HasValue)
        {
            query = query.Where(a => a.Doors <= filter.MaxDoors.Value);
        }

        // Фильтр по мощности
        if (filter.MinHorsepower.HasValue)
        {
            query = query.Where(a => a.Horsepower >= filter.MinHorsepower.Value);
        }
        if (filter.MaxHorsepower.HasValue)
        {
            query = query.Where(a => a.Horsepower <= filter.MaxHorsepower.Value);
        }

        // Фильтр по цене
        if (filter.PriceFrom.HasValue)
        {
            query = query.Where(a => a.DailyPrice >= filter.PriceFrom.Value);
        }
        if (filter.PriceTo.HasValue)
        {
            query = query.Where(a => a.DailyPrice <= filter.PriceTo.Value);
        }

        // Фильтр по статусу
        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        // Фильтр по локации (если есть координаты)
        if (filter.Latitude.HasValue && filter.Longitude.HasValue && filter.RadiusKm.HasValue)
        {
            // Применяем фильтр по расстоянию (в памяти, так как SQL не поддерживает вычисления расстояний напрямую)
            var assetsWithCoords = await query
                .Where(a => a.Latitude.HasValue && a.Longitude.HasValue)
                .ToListAsync(ct);
                
            var filteredByDistance = assetsWithCoords
                .Where(a => CalculateDistance(
                    filter.Latitude.Value, 
                    filter.Longitude.Value, 
                    a.Latitude.Value, 
                    a.Longitude.Value) <= filter.RadiusKm.Value)
                .ToList();
                
            return filteredByDistance;
        }
        else if (filter.LocationId.HasValue)
        {
            query = query.Where(a => a.LocationId == filter.LocationId.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<Asset?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.Assets
            .Include(a => a.Photos)
            .Include(a => a.Bookings)
            .FirstOrDefaultAsync(a => a.Code == code, ct);
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Assets
            .Include(a => a.Photos)
            .Include(a => a.Bookings)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IReadOnlyList<Asset>> GetPopularAssetsAsync(int limit, CancellationToken ct = default)
    {
        // Здесь можно реализовать логику популярности на основе количества бронирований
        return await _context.Assets
            .Include(a => a.Photos)
            .Include(a => a.Bookings)
            .OrderByDescending(a => a.Bookings.Count)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Asset>> GetNearbyAssetsAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        CancellationToken ct = default)
    {
        var assets = await _context.Assets
            .Include(a => a.Photos)
            .Where(a => a.Latitude.HasValue && a.Longitude.HasValue)
            .ToListAsync(ct);
            
        return assets
            .Where(a => CalculateDistance(latitude, longitude, a.Latitude.Value, a.Longitude.Value) <= radiusKm)
            .ToList();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Радиус Земли в км
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRad(double degrees) => degrees * Math.PI / 180;
}