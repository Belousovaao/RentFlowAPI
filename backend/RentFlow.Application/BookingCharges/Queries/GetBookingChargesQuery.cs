using MediatR;
using RentFlow.Application.BookingCharges.Dtos;
namespace RentFlow.Application.BookingCharges.Queries;
public sealed record GetBookingChargesQuery(Guid BookingId, Guid? CustomerId = null) : IRequest<IReadOnlyList<BookingChargeDto>>;