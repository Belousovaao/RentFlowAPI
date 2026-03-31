using RentFlow.Application.Bookings.Dtos;
using MediatR;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Commands;
public sealed record CreateBookingCommand(
    Guid AssetId,
    Guid CustomerId,
    DateTime StartDate,
    DateTime EndDate,
    DriverDto? Driver
) : IRequest<Booking>;