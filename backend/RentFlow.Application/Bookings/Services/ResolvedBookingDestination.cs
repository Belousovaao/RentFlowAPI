using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Services;

public sealed record ResolvedBookingDestination(
    BookingDestination Destination,
    decimal RouteSurcharge);