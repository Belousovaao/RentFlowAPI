using RentFlow.Domain.Locations;

namespace RentFlow.Application.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Location>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default);
}
