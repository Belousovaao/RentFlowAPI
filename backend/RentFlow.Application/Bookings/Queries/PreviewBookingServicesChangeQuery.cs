using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;

public sealed record PreviewBookingServicesChangeQuery(
    Guid BookingId,
    List<BookingServiceRequestDto> Services,
    Guid? CustomerId
) : IRequest<BookingServicesChangePreviewDto>;