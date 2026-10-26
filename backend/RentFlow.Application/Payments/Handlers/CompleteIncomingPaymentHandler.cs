using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Commands;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Handlers;

public sealed class CompleteIncomingPaymentHandler
    : IRequestHandler<CompleteIncomingPaymentCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CompleteIncomingPaymentHandler(
        IBookingRepository bookings,
        IPaymentRepository payments,
        IUnitOfWork uow,
        IDateTimeProvider dateTimeProvider)
    {
        _bookings = bookings;
        _payments = payments;
        _uow = uow;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        CompleteIncomingPaymentCommand request,
        CancellationToken ct)
    {
        if (request.PaymentId == Guid.Empty)
            throw new ArgumentException("PaymentId cannot be empty.");

        var bookingId = await _payments.GetBookingIdByPaymentIdAsync(
            request.PaymentId, ct)
            ?? throw new KeyNotFoundException("Payment not found.");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            await _uow.LockBookingPaymentsAsync(bookingId, ct);

            var booking = await _bookings.GetByIdAsync(
                bookingId, ct)
                ?? throw new KeyNotFoundException("Booking not found.");

            var payment = await _payments.GetByIdAsync(
                request.PaymentId, ct)
                ?? throw new KeyNotFoundException("Payment not found.");

            if (payment.BookingId != bookingId)
            {
                throw new InvalidOperationException(
                    "Payment booking has changed.");
            }

            if (payment.Type is not (
                PaymentType.Prepayment or PaymentType.AdditionalPayment))
            {
                throw new InvalidOperationException(
                    "Only incoming payments can be completed by this operation.");
            }

            EnsurePaymentAllowed(booking.Status, payment.Type);

            payment.Complete(
                _dateTimeProvider.UtcNow,
                externalOperationId: request.ExternalOperationId);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
        }
        catch
        {
            await _uow.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private static void EnsurePaymentAllowed(
        BookingStatus status,
        PaymentType type)
    {
        var allowed = type switch
        {
            PaymentType.Prepayment =>
                status == BookingStatus.Confirmed,

            PaymentType.AdditionalPayment =>
                status is BookingStatus.Confirmed or BookingStatus.Active,

            _ => false
        };

        if (!allowed)
        {
            throw new InvalidOperationException(
                $"Cannot complete a {type} payment for a booking " +
                $"with status '{status}'.");
        }
    }
}
