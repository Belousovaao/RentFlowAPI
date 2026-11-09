namespace RentFlow.Domain.Settings;

public sealed class RentalTariff
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private RentalTariff() { }

    public static RentalTariff Create(
        string code,
        decimal price,
        string unit,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Trim().Length > 100)
            throw new ArgumentException("A valid tariff code is required.", nameof(code));
        if (price < 0 || price != decimal.Round(price, 2))
            throw new ArgumentOutOfRangeException(nameof(price));
        if (string.IsNullOrWhiteSpace(unit) || unit.Trim().Length > 20)
            throw new ArgumentException("A valid tariff unit is required.", nameof(unit));

        return new RentalTariff
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            Price = price,
            Unit = unit.Trim(),
            UpdatedAtUtc = updatedAtUtc
        };
    }

    public void UpdatePrice(decimal price, DateTimeOffset updatedAtUtc)
    {
        if (price < 0 || price != decimal.Round(price, 2))
            throw new ArgumentOutOfRangeException(nameof(price));

        Price = price;
        UpdatedAtUtc = updatedAtUtc;
    }
}
