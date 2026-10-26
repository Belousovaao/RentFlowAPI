using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Persistance.Repositories;

public class DestinationRepository : IDestinationRepository
{
    private readonly RentFlowDbContext _db;

    public DestinationRepository(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Destination?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<Destination>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Destination>> GetActiveByTypeAsync(
        DestinationType type,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<Destination>()
            .Where(x => x.IsActive && x.Type == type)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Destination?> GetActiveByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Destinations
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsActive,
                cancellationToken);
    }
}