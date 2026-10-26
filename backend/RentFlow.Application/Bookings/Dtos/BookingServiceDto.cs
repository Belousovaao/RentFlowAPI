using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Application.Bookings.Dtos;
public sealed record BookingServiceDto(
    Guid AdditionalServiceId,
    ServiceType Type,
    string Name,
    decimal Price,
    TimeOnly? SelectedTime
);