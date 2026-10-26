public record RentalPriceSegmentDto(
    int Days,
    decimal BaseDailyPrice,
    decimal DurationDiscountPercent,
    decimal DiscountedDailyPrice,
    decimal BaseRentalPrice,
    decimal DiscountAmount,
    decimal Total,
    int AppliedForDays
);