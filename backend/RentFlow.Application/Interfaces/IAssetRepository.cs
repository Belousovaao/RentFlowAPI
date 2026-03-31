using System;
using RentFlow.Domain.Assets;
using RentFlow.Application.Assets.Queries;

namespace RentFlow.Application.Interfaces;

public interface IAssetRepository
{
    Task<IReadOnlyList<Asset>> SearchAsync(AssetFilter filter, CancellationToken ct = default);
    Task<Asset?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
