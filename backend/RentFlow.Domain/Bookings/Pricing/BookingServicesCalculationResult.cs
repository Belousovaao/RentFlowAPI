
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Domain.Bookings.Pricing;

public sealed record BookingServicesCalculationResult(decimal servicePrice, IReadOnlyCollection<CalculatedService> Services
);