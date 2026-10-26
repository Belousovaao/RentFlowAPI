namespace RentFlow.Domain.Bookings;
public sealed class BookingChargeSettings
{
    public int Id { get; private set; }
    public decimal LateCancellationTariff { get; private set; }
    public decimal RefuelingPricePerLiter { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    private BookingChargeSettings() { }
    public static BookingChargeSettings CreateDefault(DateTimeOffset now) => new() { Id = 1, UpdatedAt = now };
    public void Update(decimal lateCancellationTariff, decimal refuelingPricePerLiter, DateTimeOffset updatedAt, Guid? updatedByUserId = null)
    {
        if (lateCancellationTariff < 0 || lateCancellationTariff != decimal.Round(lateCancellationTariff, 2)) throw new ArgumentOutOfRangeException(nameof(lateCancellationTariff));
        if (refuelingPricePerLiter < 0 || refuelingPricePerLiter != decimal.Round(refuelingPricePerLiter, 2)) throw new ArgumentOutOfRangeException(nameof(refuelingPricePerLiter));
        LateCancellationTariff = lateCancellationTariff; RefuelingPricePerLiter = refuelingPricePerLiter; UpdatedAt = updatedAt; UpdatedByUserId = updatedByUserId;
    }
}