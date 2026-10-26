using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Queries;
public sealed record CalculateBookingPriceQuery(
    Guid AssetId,

    DateOnly PickupDate,
    TimeOnly? PickupTime,

    DateOnly ReturnDate,
    TimeOnly? ReturnTime,

    List<BookingServiceRequestDto> Services,
    DestinationType DestinationType,
    Guid? DestinationId,
    string? CustomRouteDescription
) : IRequest<BookingCalculationResultDto>;