using RentFlow.Application.Bookings.Services;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Interfaces;

public interface IBookingDestinationResolver
{
    Task<ResolvedBookingDestination> ResolveAsync(
        DestinationType type,
        Guid? destinationId,
        string? customRouteDescription,
        CancellationToken cancellationToken = default);
}