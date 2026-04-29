using RentFlow.Domain.Assets;

namespace RentFlow.Application.Interfaces;
public interface IAssetPhotoRepository
{
    Task AddAsync(AssetPhoto photo, CancellationToken ct);
    Task<AssetPhoto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task DeleteAsync(AssetPhoto photo, CancellationToken ct);
}