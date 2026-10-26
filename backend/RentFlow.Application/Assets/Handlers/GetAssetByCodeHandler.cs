using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Mappers;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;

public sealed class GetAssetByCodeHandler : IRequestHandler<GetAssetByCodeQuery, AssetDto?>
{
    private readonly IAssetRepository _repo;
    private readonly ILocationRepository _locationRepo;

    public GetAssetByCodeHandler(
        IAssetRepository repo,
        ILocationRepository locationRepo)
    {
        _repo = repo;
        _locationRepo = locationRepo;
    }

    public async Task<AssetDto?> Handle(GetAssetByCodeQuery request, CancellationToken cancellationToken)
    {
        var asset = await _repo.GetByCodeAsync(request.Code, cancellationToken);

        if (asset == null)
            return null;

        var location = await _locationRepo.GetByIdAsync(asset.LocationId, cancellationToken);
        var photos = asset.Photos.Select(p => p.Url).ToList();

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

        return AssetMapper.ToDto(asset) with
        {
            LocationName = location?.Name ?? string.Empty,
            Photos = allPhotos,
            ExteriorPhotos = exteriorPhotos,
            InteriorPhotos = interiorPhotos,
        };
    }
}