namespace RentFlow.Application.Bookings.Dtos;
public sealed record BookingCalculationResultDto(BookingPriceDto Price, IReadOnlyList<BookingServiceDto> Services);