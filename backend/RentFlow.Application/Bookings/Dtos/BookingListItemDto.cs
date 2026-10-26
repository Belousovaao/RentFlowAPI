using RentFlow.Application.Assets.Dtos;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Bookings.ValueObjects;
    
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Dtos;
/// <summary>
/// Клиент: мои бронирования - краткая информация о бронировании для отображения в ЛK клиента
/// </summary>
/// <param name="Id"></param>
/// <param name="Asset"></param>
/// <param name="RentalPeriod"></param>
/// <param name="Price"></param>
/// <param name="Status"></param>
/// <param name="Services"></param>
public sealed record BookingListItemDto(
    Guid Id,
    AssetShortDto Asset,
    RentalPeriod RentalPeriod,
    BookingPriceDto Price,
    BookingStatus Status,
    IReadOnlyCollection<BookingServiceDto> Services
);