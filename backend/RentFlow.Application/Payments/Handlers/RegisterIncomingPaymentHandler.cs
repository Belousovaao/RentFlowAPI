using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Commands;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Handlers;

public sealed class RegisterIncomingPaymentHandler
    : IRequestHandler<RegisterIncomingPaymentCommand, Guid>
{
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterIncomingPaymentHandler(
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

    public async Task<Guid> Handle(
        RegisterIncomingPaymentCommand request,
        CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty)
            throw new ArgumentException("BookingId cannot be empty.");

        if (request.Amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "Payment amount must be greater than zero.");

        if (request.Amount != decimal.Round(request.Amount, 2))
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "Payment amount cannot have more than two decimal places.");

        if (request.Type is not (
            PaymentType.Prepayment or PaymentType.AdditionalPayment))
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Type),
                "Only incoming payment types are allowed.");
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            await _uow.LockBookingPaymentsAsync(
                request.BookingId, ct);

            var booking = await _bookings.GetByIdAsync(
                request.BookingId, ct)
                ?? throw new KeyNotFoundException("Booking not found.");

            EnsurePaymentAllowed(booking.Status, request.Type);

            var payment = Payment.CreateIncoming(
                request.BookingId,
                request.Amount,
                request.Type,
                request.Method,
                _dateTimeProvider.UtcNow,
                note: request.Note);

            await _payments.AddAsync(payment, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            return payment.Id;
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
                $"Cannot register a {type} payment for a booking " +
                $"with status '{status}'.");
        }
    }
}
