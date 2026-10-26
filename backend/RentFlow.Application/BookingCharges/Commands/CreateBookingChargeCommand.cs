using MediatR;
using RentFlow.Domain.Bookings;
namespace RentFlow.Application.BookingCharges.Commands;
public sealed record CreateBookingChargeCommand(Guid BookingId, BookingChargeType Type, decimal? Amount = null, decimal? Quantity = null, string? Title = null, string? Description = null, string? ReferenceNumber = null) : IRequest<Guid>;