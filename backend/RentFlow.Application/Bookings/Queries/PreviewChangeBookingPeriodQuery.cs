using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;

public record PreviewChangeBookingPeriodQuery(
    Guid BookingId,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? CustomerId
) : IRequest<ChangeBookingPeriodPreviewDto>;
