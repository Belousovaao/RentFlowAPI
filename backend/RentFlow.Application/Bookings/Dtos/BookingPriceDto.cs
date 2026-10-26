namespace RentFlow.Application.Bookings.Dtos;

public record BookingPriceDto(
    RentalPriceDto RentalPrice,
    decimal Deposit,
    decimal ServicesPrice,
    decimal RouteSurcharge,
    decimal Total,
    decimal TotalToPay,
    IReadOnlyCollection<BookingPenaltyDto> Penalties,
    decimal PenaltiesPrice
);