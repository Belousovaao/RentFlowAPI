using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Domain.Bookings.ValueObjects;

public sealed record SelectedAdditionalService(
    AdditionalService Service,
    TimeOnly? SelectedTime
);