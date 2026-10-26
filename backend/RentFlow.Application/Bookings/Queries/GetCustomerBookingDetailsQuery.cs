using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;

public sealed record GetCustomerBookingDetailsQuery(
    Guid CustomerId,
    Guid BookingId
) : IRequest<BookingDetailsDto?>;