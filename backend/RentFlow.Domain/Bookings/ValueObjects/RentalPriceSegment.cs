public class RentalPriceSegment
{
    public Guid Id { get; private set; }
    public int Order { get; private set; }
    /// <summary>
    /// Количество суток.
    /// </summary>
    public int Days { get; private set; }

    /// <summary>
    /// Базовая цена автомобиля за сутки.
    /// </summary>
    public decimal BaseDailyPrice { get; private set; }

    /// <summary>
    /// Скидка за длительность аренды, %.
    /// </summary>
    public decimal DurationDiscountPercent { get; private set; }

    /// <summary>
    /// Цена суток с учетом скидки.
    /// </summary>
    public decimal DiscountedDailyPrice { get; private set; }

    /// <summary>
    /// Стоимость аренды без скидки.
    /// </summary>
    public decimal BaseRentalPrice { get; private set; }

    /// <summary>
    /// Размер скидки в рублях.
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Стоимость данного сегмента со скидкой.
    /// </summary>
    public decimal Total { get; private set; }

    /// <summary>
    /// Общий срок аренды, на основании которого был определен тариф.
    /// Например, при продлении с 6 до 9 суток:
    /// Days = 3, AppliedForDays = 9.
    /// </summary>
    public int AppliedForDays { get; private set; }

    private RentalPriceSegment()
    {
        
    }

    public RentalPriceSegment(
        int days,
        decimal baseDailyPrice,
        decimal durationDiscountPercent,
        decimal discountedDailyPrice,
        decimal baseRentalPrice,
        decimal discountAmount,
        decimal total,
        int appliedForDays,
        int order)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days));

        if (appliedForDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(appliedForDays));

        if (baseDailyPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(baseDailyPrice));

        if (discountedDailyPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(discountedDailyPrice));
        Id = Guid.NewGuid();
        Order = order;
        Days = days;
        BaseDailyPrice = baseDailyPrice;
        DurationDiscountPercent = durationDiscountPercent;
        DiscountedDailyPrice = discountedDailyPrice;
        BaseRentalPrice = baseRentalPrice;
        DiscountAmount = discountAmount;
        Total = total;
        AppliedForDays = appliedForDays;
    }

    public RentalPriceSegment Copy()
    {
        return new RentalPriceSegment(
            days: Days,
            baseDailyPrice: BaseDailyPrice,
            durationDiscountPercent: DurationDiscountPercent,
            discountedDailyPrice: DiscountedDailyPrice,
            baseRentalPrice: BaseRentalPrice,
            discountAmount: DiscountAmount,
            total: Total,
            appliedForDays: AppliedForDays,
            order: Order);
    }

    internal void SetOrder(int order)
    {
        Order = order;
    }

    public void Update(
        int days,
        decimal durationDiscountPercent,
        decimal discountedDailyPrice,
        decimal baseRentalPrice,
        decimal discountAmount,
        decimal total,
        int appliedForDays)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days));

        if (appliedForDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(appliedForDays));

        if (discountedDailyPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(discountedDailyPrice));
        Days = days;
        DurationDiscountPercent = durationDiscountPercent;
        DiscountedDailyPrice = discountedDailyPrice;
        BaseRentalPrice = baseRentalPrice;
        DiscountAmount = discountAmount;
        Total = total;
        AppliedForDays = appliedForDays;
    }
}