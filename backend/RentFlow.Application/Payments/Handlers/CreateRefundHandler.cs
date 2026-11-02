using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Commands;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Handlers;

public sealed class CreateRefundHandler : IRequestHandler<CreateRefundCommand, Guid>
{
    private readonly IPaymentRepository _payments;
    private readonly IPaymentAllocationRepository _allocations;
    private readonly IBookingChargeRepository _charges;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateRefundHandler(
        IPaymentRepository payments,
        IPaymentAllocationRepository allocations,
        IBookingChargeRepository charges,
        IUnitOfWork uow,
        IDateTimeProvider dateTimeProvider)
    {
        _payments = payments;
        _allocations = allocations;
        _charges = charges;
        _uow = uow;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Guid> Handle(
        CreateRefundCommand request,
        CancellationToken ct)
    {
        if (request.OriginalPaymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "OriginalPaymentId cannot be empty.");
        }

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "Refund amount must be greater than zero.");
        }

        if (decimal.Round(request.Amount, 2) != request.Amount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "Refund amount cannot have more than two decimal places.");
        }

        var bookingId = await _payments.GetBookingIdByPaymentIdAsync(
            request.OriginalPaymentId,
            ct)
            ?? throw new KeyNotFoundException("Original payment not found.");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            await _uow.LockBookingPaymentsAsync(bookingId, ct);

            // Загружаем исходный платёж после блокировки.
            var originalPayment = await _payments.GetByIdAsync(
                request.OriginalPaymentId,
                ct)
                ?? throw new KeyNotFoundException("Original payment not found.");

            if (originalPayment.BookingId != bookingId)
            {
                throw new InvalidOperationException(
                    "Original payment booking does not match.");
            }

            if (originalPayment.Type is not (
                PaymentType.Prepayment or PaymentType.AdditionalPayment))
            {
                throw new InvalidOperationException(
                    "Refund must reference an incoming payment.");
            }

            if (originalPayment.Status != PaymentStatus.Completed)
            {
                throw new InvalidOperationException(
                    "Only completed incoming payments can be refunded.");
            }

            var payments = await _payments.GetByBookingIdAsync(
                bookingId,
                ct);

            var refunds = payments
                .Where(p =>
                    p.Type == PaymentType.Refund &&
                    p.OriginalPaymentId == originalPayment.Id &&
                    p.Status is PaymentStatus.Pending or PaymentStatus.Completed)
                .Sum(p => p.Amount);

            var bookingAllocations = await _allocations.GetByBookingIdAsync(bookingId, ct);
            var allocatedAmount = bookingAllocations
                .Where(x => x.PaymentId == originalPayment.Id)
                .Sum(x => x.Amount);
            var paymentUnallocated = originalPayment.Amount - refunds - allocatedAmount;

            var incomingPayments = payments
                .Where(p => p.Status == PaymentStatus.Completed &&
                    p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment)
                .ToList();
            var totalUnallocated = incomingPayments.Sum(p => p.Amount) -
                payments.Where(p => p.Type == PaymentType.Refund &&
                    p.Status is PaymentStatus.Pending or PaymentStatus.Completed).Sum(p => p.Amount) -
                bookingAllocations.Where(a => incomingPayments.Any(p => p.Id == a.PaymentId)).Sum(a => a.Amount);

            var charges = await _charges.GetByBookingIdAsync(bookingId, ct);
            var allocatedByCharge = bookingAllocations
                .GroupBy(a => a.BookingChargeId)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.Amount));
            var outstandingDebt = charges
                .Where(c => c.Status == RentFlow.Domain.Bookings.BookingChargeStatus.Accrued)
                .Sum(c => Math.Max(0m, c.Amount - allocatedByCharge.GetValueOrDefault(c.Id)));

            var availableToRefund = Math.Min(paymentUnallocated, Math.Max(0m, totalUnallocated - outstandingDebt));

            if (request.Amount > availableToRefund)
            {
                throw new InvalidOperationException(
                    $"Refund exceeds the available amount. " +
                    $"Available: {availableToRefund:F2} RUB.");
            }

            var refund = Payment.CreateRefund(
                bookingId,
                originalPayment.Id,
                request.Amount,
                originalPayment.Method,
                _dateTimeProvider.UtcNow,
                note: request.Note);

            refund.Complete(_dateTimeProvider.UtcNow);\n            await _payments.AddAsync(refund, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            return refund.Id;
        }
        catch
        {
            await _uow.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}