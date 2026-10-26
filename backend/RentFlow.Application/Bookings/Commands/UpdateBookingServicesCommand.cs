
using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Commands;
public sealed record UpdateBookingServicesCommand(
    Guid BookingId,
    List<BookingServiceRequestDto> Services,
    Guid? CustomerId
) : IRequest;