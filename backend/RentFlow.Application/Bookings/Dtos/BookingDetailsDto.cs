using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record BookingDetailsDto(
    Guid Id,
    BookingStatus Status,
    Guid CustomerId,
    CustomerShortDto Customer,
    Guid AssetId,
    AssetShortDto Asset,
    RentalPeriod RentalPeriod,
    BookingPriceDto Price,
    IReadOnlyList<BookingServiceDto> Services,
    BookingDestinationDto Destination,
    DestinationDto? DestinationDetails
);