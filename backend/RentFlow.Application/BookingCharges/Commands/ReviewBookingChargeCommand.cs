using MediatR;
namespace RentFlow.Application.BookingCharges.Commands;
public sealed record ReviewBookingChargeCommand(Guid BookingId, Guid ChargeId, bool Confirm) : IRequest;