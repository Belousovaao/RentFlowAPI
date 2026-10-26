
namespace RentFlow.Domain.Payments;

public sealed class Payment
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }

    // Для возврата — идентификатор исходного поступления.
    public Guid? OriginalPaymentId { get; private set; }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";

    public PaymentType Type { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }

    // Идентификатор операции во внешней платёжной системе.
    public string? ExternalOperationId { get; private set; }

    // Кто создал или подтвердил операцию вручную.
    public Guid? CreatedByUserId { get; private set; }
    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public string? Note { get; private set; }

    private Payment() { }

    private Payment(
        Guid bookingId,
        decimal amount,
        PaymentType type,
        PaymentMethod method,
        DateTimeOffset createdAt,
        Guid? originalPaymentId,
        Guid? createdByUserId,
        string? note)
    {
        if (bookingId == Guid.Empty)
            throw new ArgumentException("Booking ID cannot be empty.", nameof(bookingId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (type == PaymentType.Refund &&
            originalPaymentId is null)
        {
            throw new ArgumentException("A refund must reference its original payment.", nameof(originalPaymentId));
        }

        if (type != PaymentType.Refund &&
            originalPaymentId is not null)
        {
            throw new ArgumentException("Only refunds can reference an original payment.", nameof(originalPaymentId));
        }

        Id = Guid.NewGuid();
        BookingId = bookingId;
        Amount = amount;
        Type = type;
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = createdAt;
        OriginalPaymentId = originalPaymentId;
        CreatedByUserId = createdByUserId;
        Note = note;
    }

    public static Payment CreateIncoming(
        Guid bookingId,
        decimal amount,
        PaymentType type,
        PaymentMethod method,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null,
        string? note = null)
    {
        if (type is not (PaymentType.Prepayment or PaymentType.AdditionalPayment))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        return new Payment(
            bookingId,
            amount,
            type,
            method,
            createdAt,
            originalPaymentId: null,
            createdByUserId,
            note);
    }

    public static Payment CreateRefund(
        Guid bookingId,
        Guid originalPaymentId,
        decimal amount,
        PaymentMethod method,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null,
        string? note = null)
    {
        return new Payment(
            bookingId,
            amount,
            PaymentType.Refund,
            method,
            createdAt,
            originalPaymentId,
            createdByUserId,
            note);
    }

    public void Complete(
        DateTimeOffset completedAt,
        Guid? completedByUserId = null,
        string? externalOperationId = null)
    {
        EnsurePending();

        if (completedAt < CreatedAt)
            throw new ArgumentOutOfRangeException(nameof(completedAt));

        Status = PaymentStatus.Completed;
        CompletedAt = completedAt;
        CompletedByUserId = completedByUserId;
        ExternalOperationId = externalOperationId;
    }

    public void Fail()
    {
        EnsurePending();
        Status = PaymentStatus.Failed;
    }

    public void Cancel()
    {
        EnsurePending();
        Status = PaymentStatus.Cancelled;
    }

    private void EnsurePending()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can change status.");
    }
}

public enum PaymentType
{
    Prepayment,
    AdditionalPayment,
    Refund
}

public enum PaymentMethod
{
    BankTransfer,
    Online
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}
