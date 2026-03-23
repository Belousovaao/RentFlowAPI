using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Queries;

public record GetBookingsQuery() : IRequest<List<BookingDto>>;
