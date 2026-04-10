using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Locations;

namespace RentFlow.Persistance.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly RentFlowDbContext _context;

    public LocationRepository(RentFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    public async Task<IReadOnlyList<Location>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await _context.Locations
            .Where(l => ids.Contains(l.Id))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Locations
            .ToListAsync(ct);
    }
}