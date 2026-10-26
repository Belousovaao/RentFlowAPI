namespace RentFlow.Application.Bookings.Dtos;

public sealed record BookingServicesChangePreviewDto(
    decimal CurrentServicesPrice,
    decimal NewServicesPrice,
    decimal Difference,
    decimal CurrentTotal,
    decimal NewTotal,
    IReadOnlyCollection<BookingServiceDto> Services
);