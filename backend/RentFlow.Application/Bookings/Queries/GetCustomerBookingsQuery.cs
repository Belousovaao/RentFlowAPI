using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Queries;

public record GetCustomerBookingsQuery(Guid CustomerId) : IRequest<IReadOnlyList<BookingListItemDto>>;
