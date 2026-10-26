using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Persistance.Repositories;

public class AdditionalServiceRepository
    : IAdditionalServiceRepository
{
    private readonly RentFlowDbContext _db;

    public AdditionalServiceRepository(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task<List<AdditionalService>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _db.Set<AdditionalService>()
            .Where(x => x.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(ct);
    }

    public async Task<List<AdditionalService>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await _db.Set<AdditionalService>()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }
}