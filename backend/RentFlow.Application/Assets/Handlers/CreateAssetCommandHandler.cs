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
            shortDescription: "",
            fullDescription: "",
            type: AssetType.Auto,
            category: AssetCategory.Crossover,
            year: request.Year,
            fuelType: request.FuelType,
            transmission: request.Transmission,
            driveType: request.DriveType,
            seats: request.Seats,
            doors: 5,
            engine: "",
            horsepower: request.Horsepower,
            acceleration: "",
            topSpeed: 60,
            color: "",
            features: new List<string>(),
            dailyPrice: request.DailyPrice,
            deposit: 5000,
            locationId: Guid.Empty,
            canDeliver: false,
            deliveryPrice: null,
            latitude: null,
            longitude: null
            );
            asset.UpdateStatus(request.Status);
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