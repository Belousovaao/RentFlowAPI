public class BookingPenalty
{
    public Guid Id { get; private set; }
    public BookingPenaltyType Type { get; private set; }
    public decimal Amount { get; private set; }

    private BookingPenalty()
    {
    }

    private BookingPenalty(
        BookingPenaltyType type,
        decimal amount)
    {
        Id = Guid.NewGuid();
        Type = type;
        Amount = amount;
    }

    public static BookingPenalty PeriodReduction(decimal amount) => new(BookingPenaltyType.PeriodReduction, amount);
}

public enum BookingPenaltyType
{
    PeriodReduction
}