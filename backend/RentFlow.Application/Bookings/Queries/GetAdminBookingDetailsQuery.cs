using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;

public record GetAdminBookingDetailsQuery(Guid BookingId) : IRequest<AdminBookingDetailsDto>;