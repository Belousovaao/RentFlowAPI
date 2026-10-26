using RentFlow.Application.Bookings.Dtos;
using MediatR;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Commands;
public sealed record CreateBookingCommand(
    Guid AssetId,
    Guid CustomerId,

    DateOnly PickupDate,
    TimeOnly? PickupTime,

    DateOnly ReturnDate,
    TimeOnly? ReturnTime,

    DriverDto? Driver,

    List<BookingServiceRequestDto> Services,
    DestinationType DestinationType,
    Guid? DestinationId,
    string? CustomRouteDescription
) : IRequest<Booking>;