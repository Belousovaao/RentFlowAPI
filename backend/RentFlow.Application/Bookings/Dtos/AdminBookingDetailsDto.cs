using RentFlow.Application.Assets.Dtos;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Dtos;
/// <summary>
/// Админ: подробная информация о бронировании для отображения в ЛK админа
/// </summary>
/// <param name="Id"></param>
/// <param name="Status"></param>
/// <param name="Asset"></param>
/// <param name="RentalPeriod"></param>
/// <param name="Price"></param>
/// <param name="Services"></param>
/// <param name="Customer"></param>
public record AdminBookingDetailsDto(
    Guid Id,
    BookingStatus Status,
    AssetShortDto Asset,
    RentalPeriod RentalPeriod,
    BookingPriceDto Price,
    IReadOnlyList<BookingServiceDto> Services,
    AdminCustomerDetailsDto Customer,
    BookingDestinationDto Destination,
    DestinationDto? DestinationDetails
);