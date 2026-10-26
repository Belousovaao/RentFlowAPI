using MediatR;
namespace RentFlow.Application.BookingCharges.Commands;
public sealed record PaymentAllocationItem(Guid BookingChargeId, decimal Amount);
public sealed record AllocatePaymentToChargesCommand(Guid PaymentId, IReadOnlyList<PaymentAllocationItem> Allocations) : IRequest;