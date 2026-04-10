using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Assets.Handlers;

public sealed class SearchAssetsHandler : IRequestHandler<SearchAssetsQuery, PaginatedResponse<AssetDto>>
{
    private readonly IAssetRepository _repo;
    private readonly ILocationRepository _locationRepo;
    private readonly IBookingRepository _bookingRepo;

    public SearchAssetsHandler(
        IAssetRepository repo,
        ILocationRepository locationRepo,
        IBookingRepository bookingRepo)
    {
        _repo = repo;
        _locationRepo = locationRepo;
        _bookingRepo = bookingRepo;
    }

    public async Task<PaginatedResponse<AssetDto>> Handle(SearchAssetsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        
        // Получаем все активы с фильтрацией
        var assets = await _repo.SearchAsync(filter, cancellationToken);
        
        // Фильтрация по доступности на выбранные даты
        if (filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            assets = await FilterAvailableAssets(assets, filter.StartDate.Value, filter.EndDate.Value, cancellationToken);
        }
        
        // Сортировка
        assets = ApplySorting(assets, filter.SortBy);
        
        // Подсчет общего количества до пагинации
        var total = assets.Count();
        
        // Пагинация
        var pagedAssets = assets
            .Skip((filter.Page - 1) * filter.Limit)
            .Take(filter.Limit)
            .ToList();
        
        // Получаем информацию о локациях
        var locationIds = pagedAssets.Select(a => a.LocationId).Distinct();
        var locations = await _locationRepo.GetByIdsAsync(locationIds, cancellationToken);
        var locationDict = locations.ToDictionary(l => l.Id, l => l);
        
        // Конвертируем в DTO
        var dtos = new List<AssetDto>();
        foreach (var asset in pagedAssets)
        {
            var location = locationDict.GetValueOrDefault(asset.LocationId);
            var distance = CalculateDistance(
                filter.Latitude, filter.Longitude,
                asset.Latitude, asset.Longitude
            );
            
            var photos = asset.Photos.Select(p => p.Url).ToList();
            var isAvailable = asset.Status == AssetStatus.Available;
            
            // Если есть фильтр по датам, проверяем еще раз
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                isAvailable = await _bookingRepo.IsAssetAvailableAsync(
                    asset.Id, 
                    filter.StartDate.Value, 
                    filter.EndDate.Value,
                    cancellationToken
                );
            }
            
            dtos.Add(new AssetDto(
                asset.Id,
                asset.Code,
                asset.BrandName,
                asset.Model,
                asset.Year,
                asset.ShortDescription,
                asset.FullDescription,
                asset.Type,
                asset.Category,
                asset.FuelType,
                asset.Transmission,
                asset.DriveType,
                asset.Seats,
                asset.Doors,
                asset.Engine,
                asset.Horsepower,
                asset.Acceleration,
                asset.TopSpeed,
                asset.Color,
                asset.Features,
                asset.DailyPrice,
                asset.Deposit,
                asset.Status,
                asset.LocationId,
                location?.Name ?? "Unknown Location",
                asset.Latitude,
                asset.Longitude,
                distance,
                photos,
                isAvailable
            ));
        }
        
        return new PaginatedResponse<AssetDto>(
            dtos,
            total,
            filter.Page,
            filter.Limit,
            (int)Math.Ceiling(total / (double)filter.Limit),
            filter.Page * filter.Limit < total,
            filter.Page > 1
        );
    }
    
    private IReadOnlyList<Asset> ApplySorting(IReadOnlyList<Asset> assets, string? sortBy)
    {
        var sorted = sortBy switch
        {
            "price_asc" => assets.OrderBy(a => a.DailyPrice),
            "price_desc" => assets.OrderByDescending(a => a.DailyPrice),
            "year_desc" => assets.OrderByDescending(a => a.Year),
            "popular" => assets.OrderByDescending(a => a.Bookings != null ? a.Bookings.Count : 0),
            _ => assets.OrderBy(a => a.BrandName).ThenBy(a => a.Model)
        };

        return sorted.ToList();
    }
    
    private double? CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
            return null;
            
        var R = 6371; // Радиус Земли в км
        var dLat = ToRad(lat2.Value - lat1.Value);
        var dLon = ToRad(lon2.Value - lon1.Value);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1.Value)) * Math.Cos(ToRad(lat2.Value)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    
    private double ToRad(double degrees) => degrees * Math.PI / 180;
    
    private async Task<IReadOnlyList<Asset>> FilterAvailableAssets(
        IReadOnlyList<Asset> assets, 
        DateTime startDate, 
        DateTime endDate,
        CancellationToken ct)
    {
        var assetIds = assets.Select(a => a.Id).ToList();
        var bookedAssetIds = await _bookingRepo.GetBookedAssetIdsAsync(assetIds, startDate, endDate, ct);
        
        return assets.Where(a => !bookedAssetIds.Contains(a.Id)).ToList();
    }
}