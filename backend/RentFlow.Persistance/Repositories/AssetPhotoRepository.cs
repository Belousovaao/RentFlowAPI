using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Persistance;

namespace RentFlow.Infrastructure.Persistence.Repositories;
public class AssetPhotoRepository : IAssetPhotoRepository
{
    private readonly RentFlowDbContext _context;
    
    public AssetPhotoRepository(RentFlowDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(AssetPhoto photo, CancellationToken ct)
    {
        await _context.AssetPhotos.AddAsync(photo, ct);
    }
    
    public async Task<AssetPhoto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.AssetPhotos.FirstOrDefaultAsync(p => p.Id == id, ct);
    }
    
    public Task DeleteAsync(AssetPhoto photo, CancellationToken ct)
    {
        _context.AssetPhotos.Remove(photo);
        return Task.CompletedTask;
    }
}