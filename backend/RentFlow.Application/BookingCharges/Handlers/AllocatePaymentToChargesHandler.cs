using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class AllocatePaymentToChargesHandler : IRequestHandler<AllocatePaymentToChargesCommand>
{
    private readonly IPaymentRepository _payments; private readonly IBookingChargeRepository _charges; private readonly IPaymentAllocationRepository _allocations; private readonly IUnitOfWork _uow; private readonly IDateTimeProvider _clock;
    public AllocatePaymentToChargesHandler(IPaymentRepository payments, IBookingChargeRepository charges, IPaymentAllocationRepository allocations, IUnitOfWork uow, IDateTimeProvider clock) { _payments = payments; _charges = charges; _allocations = allocations; _uow = uow; _clock = clock; }
    public async Task Handle(AllocatePaymentToChargesCommand request, CancellationToken ct)
    {
        if (request.PaymentId == Guid.Empty || request.Allocations is null || request.Allocations.Count == 0) throw new ArgumentException("PaymentId and at least one allocation are required.");
        if (request.Allocations.Any(x => x.BookingChargeId == Guid.Empty || x.Amount <= 0 || x.Amount != decimal.Round(x.Amount, 2))) throw new ArgumentException("Each allocation must have a charge and positive amount with at most two decimal places.");
        if (request.Allocations.Select(x => x.BookingChargeId).Distinct().Count() != request.Allocations.Count) throw new ArgumentException("A charge can appear only once in an allocation request.");
        var bookingId = await _payments.GetBookingIdByPaymentIdAsync(request.PaymentId, ct) ?? throw new KeyNotFoundException("Payment not found.");
        await _uow.BeginTransactionAsync(ct);
        try
        {
            await _uow.LockBookingPaymentsAsync(bookingId, ct);
            var payment = await _payments.GetByIdAsync(request.PaymentId, ct) ?? throw new KeyNotFoundException("Payment not found.");
            if (payment.Type is not (PaymentType.Prepayment or PaymentType.AdditionalPayment) || payment.Status != PaymentStatus.Completed) throw new InvalidOperationException("Only completed incoming payments can be allocated.");
            var current = await _allocations.GetByPaymentIdAsync(payment.Id, ct);
            var allPayments = await _payments.GetByBookingIdAsync(bookingId, ct);
            var refunds = allPayments.Where(p => p.Type == PaymentType.Refund && p.OriginalPaymentId == payment.Id && p.Status is PaymentStatus.Pending or PaymentStatus.Completed).Sum(p => p.Amount);
            var requested = request.Allocations.Sum(x => x.Amount);
            var available = payment.Amount - current.Sum(x => x.Amount) - refunds;
            if (requested > available) throw new InvalidOperationException($"Allocation exceeds unallocated payment balance. Available: {Math.Max(0m, available):F2} RUB.");
            var ids = request.Allocations.Select(x => x.BookingChargeId).ToHashSet();
            var charges = (await _charges.GetByBookingIdAsync(bookingId, ct)).Where(x => ids.Contains(x.Id)).ToList();
            if (charges.Count != ids.Count) throw new InvalidOperationException("Every allocated charge must belong to the same booking as the payment.");
            var prior = await _allocations.GetByBookingIdAsync(bookingId, ct);
            foreach (var item in request.Allocations)
            {
                var charge = charges.Single(x => x.Id == item.BookingChargeId);
                if (charge.Status != BookingChargeStatus.Confirmed) throw new InvalidOperationException("Payments can be allocated only to confirmed charges.");
                var paid = prior.Where(a => a.BookingChargeId == charge.Id).Sum(a => a.Amount);
                if (paid + item.Amount > charge.Amount) throw new InvalidOperationException($"Allocation exceeds the remaining balance for charge '{charge.Title}'.");
            }
            await _allocations.AddRangeAsync(request.Allocations.Select(x => PaymentAllocation.Create(payment.Id,x.BookingChargeId,x.Amount,_clock.UtcNow)), ct);
            await _uow.SaveChangesAsync(ct); await _uow.CommitAsync(ct);
        }
        catch { await _uow.RollbackAsync(CancellationToken.None); throw; }
    }
}