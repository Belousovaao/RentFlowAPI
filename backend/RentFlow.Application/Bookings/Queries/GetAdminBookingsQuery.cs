using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;
public record GetAdminBookingsQuery() : IRequest<IReadOnlyList<AdminBookingListItemDto>>;