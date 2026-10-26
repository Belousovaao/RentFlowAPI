namespace RentFlow.Domain.Bookings.Services;
public sealed record BookingServiceRequest(
    AdditionalService Service,
    TimeOnly? SelectedTime);