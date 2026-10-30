namespace RentFlow.Domain.Bookings;

public sealed class BookingCharge
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public BookingChargeType Type { get; private set; }
    public BookingChargeStatus Status { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = "шт.";
    public decimal UnitPrice { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "RUB";
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }

    private BookingCharge() { }

    public static BookingCharge Create(Guid bookingId, BookingChargeType type, string title,
        string? description, string? referenceNumber, decimal quantity, string unit,
        decimal unitPrice, decimal amount, DateTimeOffset createdAt, Guid? createdByUserId = null)
    {
        if (bookingId == Guid.Empty) throw new ArgumentException("BookingId is required.", nameof(bookingId));
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 160) throw new ArgumentException("A valid charge title is required.", nameof(title));
        if (description?.Length > 2000) throw new ArgumentOutOfRangeException(nameof(description));
        if (referenceNumber?.Length > 100) throw new ArgumentOutOfRangeException(nameof(referenceNumber));
        if (string.IsNullOrWhiteSpace(unit) || unit.Length > 20) throw new ArgumentException("A valid unit is required.", nameof(unit));
        if (quantity <= 0 || quantity != decimal.Round(quantity, 3)) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0 || unitPrice != decimal.Round(unitPrice, 2)) throw new ArgumentOutOfRangeException(nameof(unitPrice));
        if (amount <= 0 || amount != decimal.Round(amount, 2)) throw new ArgumentOutOfRangeException(nameof(amount));
        return new BookingCharge { Id = Guid.NewGuid(), BookingId = bookingId, Type = type,
            Status = BookingChargeStatus.Accrued, Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim(),
            Quantity = quantity, Unit = unit.Trim(), UnitPrice = unitPrice, Amount = amount,
            Currency = "RUB", CreatedAt = createdAt, CreatedByUserId = createdByUserId };
    }

    public void Reject(DateTimeOffset reviewedAt, Guid? reviewedByUserId = null)
    {
        EnsureNotRejected();
        if (reviewedAt < CreatedAt) throw new ArgumentOutOfRangeException(nameof(reviewedAt));
        Status = BookingChargeStatus.Rejected; ReviewedAt = reviewedAt; ReviewedByUserId = reviewedByUserId;
    }
    private void EnsureNotRejected()
    {
        if (Status == BookingChargeStatus.Rejected) throw new InvalidOperationException("A rejected charge cannot be changed.");
    }
}
public enum BookingChargeType { LateCancellation, TrafficFine, Cleaning, Refueling, ExcessMileage, PropertyLossOrDamage, Other }
public enum BookingChargeStatus { Accrued, Rejected }
