
using MediatR;

namespace RentFlow.Application.Bookings.Commands;

public sealed record CancelBookingCommand(
    Guid BookingId,
    string? Reason = null,
    Guid? CustomerId = null
) : IRequest;
