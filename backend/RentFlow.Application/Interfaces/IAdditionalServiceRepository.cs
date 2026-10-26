using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Application.Interfaces;

public interface IAdditionalServiceRepository
{
    Task<List<AdditionalService>> GetActiveAsync(CancellationToken ct = default);

    Task<List<AdditionalService>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}