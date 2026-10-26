using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record BookingDestinationDto(
    Guid? DestinationId,
    DestinationType Type,
    string? CustomRouteDescription,
    RouteReviewStatus ReviewStatus,
    string? DestinationNameSnapshot
);