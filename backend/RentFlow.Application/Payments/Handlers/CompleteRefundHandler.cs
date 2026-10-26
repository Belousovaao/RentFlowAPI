using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Commands;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Handlers;

public sealed class CompleteRefundHandler : IRequestHandler<CompleteRefundCommand>
{
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CompleteRefundHandler(
        IPaymentRepository payments,
        IUnitOfWork uow,
        IDateTimeProvider dateTimeProvider)
    {
        _payments = payments;
        _uow = uow;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        CompleteRefundCommand request,
        CancellationToken ct)
    {
        if (request.RefundPaymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "RefundPaymentId cannot be empty.");
        }

        var externalOperationId = request.ExternalOperationId?.Trim();

        if (externalOperationId?.Length > 200)
        {
            throw new ArgumentException(
                "ExternalOperationId cannot exceed 200 characters.");
        }

        var bookingId = await _payments.GetBookingIdByPaymentIdAsync(
            request.RefundPaymentId,
            ct)
            ?? throw new KeyNotFoundException("Refund payment not found.");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            await _uow.LockBookingPaymentsAsync(bookingId, ct);

            // Загружаем отслеживаемую сущность после блокировки.
            var refund = await _payments.GetByIdAsync(
                request.RefundPaymentId,
                ct)
                ?? throw new KeyNotFoundException("Refund payment not found.");

            if (refund.BookingId != bookingId)
            {
                throw new InvalidOperationException(
                    "Refund booking does not match.");
            }

            if (refund.Type != PaymentType.Refund)
            {
                throw new InvalidOperationException(
                    "Only refund payments can be completed by this operation.");
            }

            if (refund.Status != PaymentStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending refunds can be completed.");
            }

            refund.Complete(
                _dateTimeProvider.UtcNow,
                externalOperationId: externalOperationId);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
        }
        catch
        {
            await _uow.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}