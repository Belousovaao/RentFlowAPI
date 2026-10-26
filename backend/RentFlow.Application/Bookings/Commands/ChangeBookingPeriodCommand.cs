using MediatR;

namespace RentFlow.Application.Bookings.Commands;
public record ChangeBookingPeriodCommand(
    Guid BookingId,
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest;