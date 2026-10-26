using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Domain.Bookings.Pricing;

public sealed record CalculatedService(
    AdditionalService Service,
    decimal Price,
    TimeOnly? SelectedTime
);