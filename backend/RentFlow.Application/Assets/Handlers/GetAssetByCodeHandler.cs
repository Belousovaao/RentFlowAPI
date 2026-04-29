using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;

public sealed class GetAssetByCodeHandler : IRequestHandler<GetAssetByCodeQuery, AssetDto?>
{
    private readonly IAssetRepository _repo;
    private readonly ILocationRepository _locationRepo;
    private readonly IBookingRepository _bookingRepo;

    public GetAssetByCodeHandler(
        IAssetRepository repo,
        ILocationRepository locationRepo,
        IBookingRepository bookingRepo)
    {
        _repo = repo;
        _locationRepo = locationRepo;
        _bookingRepo = bookingRepo;
    }

    public async Task<AssetDto?> Handle(GetAssetByCodeQuery request, CancellationToken cancellationToken)
    {
        var asset = await _repo.GetByCodeAsync(request.Code, cancellationToken);

        if (asset == null)
            return null;

        var location = await _locationRepo.GetByIdAsync(asset.LocationId, cancellationToken);
        var photos = asset.Photos.Select(p => p.Url).ToList();
        
        // Проверяем доступность (нет активных бронирований)
        var isAvailable = asset.Status == AssetStatus.Available;

        var exteriorPhotos = asset.Photos
            .Where(p => p.Type == PhotoType.Exterior)
            .OrderBy(p => p.Order)
            .Select(p => new PhotoDto(p.Id, p.Url, "exterior", p.Order))
            .ToList();
            
        var interiorPhotos = asset.Photos
            .Where(p => p.Type == PhotoType.Interior)
            .OrderBy(p => p.Order)
            .Select(p => new PhotoDto(p.Id, p.Url, "interior", p.Order))
            .ToList();
        
        var allPhotos = asset.Photos
            .OrderBy(p => p.Type)
            .ThenBy(p => p.Order)
            .Select(p => new PhotoDto(p.Id, p.Url, p.Type.ToString().ToLower(), p.Order))
            .ToList();

        return new AssetDto(
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
            null, // DistanceFromUser - будет рассчитано позже, если есть координаты пользователя
            allPhotos,
            exteriorPhotos,
            interiorPhotos
        );
    }
}