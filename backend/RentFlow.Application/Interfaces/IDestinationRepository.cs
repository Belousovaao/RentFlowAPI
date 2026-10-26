
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Interfaces;

public interface IDestinationRepository
{
    Task<Destination?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Destination>> GetActiveByTypeAsync(
        DestinationType type,
        CancellationToken cancellationToken = default);

    Task<Destination?> GetActiveByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
