using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Dtos;
/// <summary>
/// Админ: список бронирований - краткая информация о бронировании для отображения в ЛK админа
/// </summary>
/// <param name="Id"></param>
/// <param name="Asset"></param>
/// <param name="Customer"></param>
/// <param name="RentalPeriod"></param>
/// <param name="Price"></param>
/// <param name="Status"></param>
public record AdminBookingListItemDto(
    Guid Id,
    AssetShortDto Asset,
    CustomerShortDto Customer,
    RentalPeriod RentalPeriod,
    BookingPriceDto Price,
    BookingStatus Status
);