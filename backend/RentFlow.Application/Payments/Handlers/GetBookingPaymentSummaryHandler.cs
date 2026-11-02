
using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Dtos;
using RentFlow.Application.Payments.Queries;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Handlers;

public sealed class GetBookingPaymentSummaryHandler : IRequestHandler<GetBookingPaymentSummaryQuery, BookingPaymentSummaryDto>
{
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly IPaymentAllocationRepository _allocations;
    private readonly IBookingChargeRepository _charges;

    public GetBookingPaymentSummaryHandler(
        IBookingRepository bookings,
        IPaymentRepository payments,
        IPaymentAllocationRepository allocations,
        IBookingChargeRepository charges)
    {
        _bookings = bookings;
        _payments = payments;
        _allocations = allocations;
        _charges = charges;
    }

    public async Task<BookingPaymentSummaryDto> Handle(
        GetBookingPaymentSummaryQuery request,
        CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty)
            throw new ArgumentException("BookingId cannot be empty.");

        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        var payments = await _payments.GetByBookingIdAsync(
            request.BookingId, ct);

        var currencies = payments
            .Select(p => p.Currency)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (currencies.Count > 1)
            throw new InvalidOperationException("A booking contains payments in different currencies.");

        var currency = currencies.SingleOrDefault() ?? "RUB";

        var received = payments
            .Where(p => p.Status == PaymentStatus.Completed &&
                        p.Type is PaymentType.Prepayment
                            or PaymentType.AdditionalPayment)
            .Sum(p => p.Amount);

        var refunded = payments
            .Where(p => p.Status == PaymentStatus.Completed &&
                        p.Type == PaymentType.Refund)
            .Sum(p => p.Amount);

        var pendingRefunds = payments
            .Where(p => p.Status == PaymentStatus.Pending &&
                        p.Type == PaymentType.Refund)
            .Sum(p => p.Amount);

        if (refunded > received)
            throw new InvalidOperationException("Completed refunds exceed completed incoming payments.");

        var allocated = (await _allocations.GetByBookingIdAsync(request.BookingId, ct))
            .Where(a => payments.Any(p => p.Id == a.PaymentId &&
                p.Status == PaymentStatus.Completed &&
                p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment))
            .Sum(a => a.Amount);

        var refundableBeforePending = received - refunded - allocated;
        if (refundableBeforePending < 0)
            throw new InvalidOperationException("Allocated payments and completed refunds exceed completed incoming payments.");

        if (pendingRefunds > refundableBeforePending)
            throw new InvalidOperationException("Pending refunds exceed the refundable balance.");

        var bookingAllocations = await _allocations.GetByBookingIdAsync(request.BookingId, ct);
        var allocatedByCharge = bookingAllocations
            .GroupBy(a => a.BookingChargeId)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Amount));
        var charges = await _charges.GetByBookingIdAsync(request.BookingId, ct);
        var outstandingDebt = charges
            .Where(c => c.Status == RentFlow.Domain.Bookings.BookingChargeStatus.Accrued)
            .Sum(c => Math.Max(0m, c.Amount - allocatedByCharge.GetValueOrDefault(c.Id)));

        return new BookingPaymentSummaryDto(
            booking.Id,
            currency,
            received,
            refunded,
            pendingRefunds,
            received - refunded,
            Math.Max(0m, refundableBeforePending - pendingRefunds - outstandingDebt));
    }
}
