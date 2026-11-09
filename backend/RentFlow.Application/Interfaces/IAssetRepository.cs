using System;
using RentFlow.Domain.Assets;
using RentFlow.Application.Assets.Queries;

namespace RentFlow.Application.Interfaces;

public interface IAssetRepository
{
    Task<IReadOnlyList<Asset>> SearchAsync(AssetFilter filter, CancellationToken ct = default);
    Task<Asset?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Asset>> GetPopularAssetsAsync(int limit, CancellationToken ct = default);
    Task<IReadOnlyList<Asset>> GetNearbyAssetsAsync(double latitude, double longitude, double radiusKm, CancellationToken ct = default);
    Task<IEnumerable<Asset>> GetAllAsync();
    Task AddAsync(Asset asset, CancellationToken ct);
    Task UpdateAsync(Asset asset, CancellationToken ct);
    Task<IReadOnlyList<AssetMaintenancePeriod>> GetMaintenancePeriodsAsync(Guid assetId, CancellationToken ct = default);
    Task AddMaintenancePeriodAsync(AssetMaintenancePeriod period, CancellationToken ct = default);
    Task<bool> HasMaintenanceOverlapAsync(Guid assetId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<bool> DeleteMaintenancePeriodAsync(Guid assetId, Guid periodId, CancellationToken ct = default);
    void Attach(Asset asset);
    
}