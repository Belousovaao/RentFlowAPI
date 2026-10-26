using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record DestinationDto(
    Guid Id,
    string Name,
    string Region,
    int DistanceKm,
    DestinationType Type,
    decimal RouteSurcharge);