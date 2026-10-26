namespace RentFlow.Domain.Payments;
public sealed class PaymentAllocation
{
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid BookingChargeId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    private PaymentAllocation() { }
    public static PaymentAllocation Create(Guid paymentId, Guid bookingChargeId, decimal amount, DateTimeOffset createdAt, Guid? createdByUserId = null)
    {
        if (paymentId == Guid.Empty) throw new ArgumentException("PaymentId is required.", nameof(paymentId));
        if (bookingChargeId == Guid.Empty) throw new ArgumentException("BookingChargeId is required.", nameof(bookingChargeId));
        if (amount <= 0 || amount != decimal.Round(amount, 2)) throw new ArgumentOutOfRangeException(nameof(amount));
        return new PaymentAllocation { Id = Guid.NewGuid(), PaymentId = paymentId, BookingChargeId = bookingChargeId, Amount = amount, CreatedAt = createdAt, CreatedByUserId = createdByUserId };
    }
}