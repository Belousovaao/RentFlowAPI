namespace RentFlow.Application.Bookings.Dtos;

public record RentalPriceDto(
    IReadOnlyList<RentalPriceSegmentDto> Segments,
    decimal BaseRentalPrice,
    decimal DiscountAmount,
    decimal RentalPriceWithDiscount
);