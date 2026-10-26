using System.Diagnostics;
using MediatR;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;
public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Asset>
{
    private readonly IAssetRepository _repo;
    private readonly IUnitOfWork _uow;
    
    public CreateAssetCommandHandler(IAssetRepository repo, IUnitOfWork uow) 
        => (_repo, _uow) = (repo, uow);

    public async Task<Asset> Handle(CreateAssetCommand request, CancellationToken ct)
    {
        try
        {
            var code = GenerateCode(request.BrandName, request.Model);

            Asset asset = Asset.Create(
            code: code,
            brandName: request.BrandName,
            model: request.Model,
            shortDescription: request.ShortDescription,
            fullDescription: request.FullDescription,
            type: request.Type,
            category: request.Category,
            year: request.Year,
            fuelType: request.FuelType,
            transmission: request.Transmission,
            driveType: request.DriveType,
            seats: request.Seats,
            doors: request.Doors,
            engine: request.Engine,
            horsepower: request.Horsepower,
            acceleration: request.Acceleration,
            topSpeed: request.TopSpeed,
            color: request.Color,
            features: request.Features,
            dailyPrice: request.DailyPrice,
            deposit: request.Deposit,
            locationId: request.LocationId ?? Guid.Empty,
            latitude: request.Latitude,
            longitude: request.Longitude
            );
            asset.UpdateStatus(Enum.Parse<AssetStatus>(request.Status));
            await _repo.AddAsync(asset, ct);
            await _uow.SaveChangesAsync(ct);
            return asset;
        }
        catch (Exception ex)
        {
            throw;
        }
        
    }

    private string GenerateCode(string brand, string model) 
        => $"{brand}_{model}_{Guid.NewGuid():N}".Substring(0, 20);
}