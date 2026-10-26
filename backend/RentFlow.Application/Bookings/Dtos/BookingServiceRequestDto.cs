namespace RentFlow.Application.Bookings.Dtos;
public sealed record BookingServiceRequestDto(
    Guid AdditionalServiceId,
    TimeOnly? SelectedTime
);