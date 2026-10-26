
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Domain.Bookings.Pricing;

public sealed record BookingCalculationResult(BookingPrice Price, IReadOnlyCollection<CalculatedService> Services
);