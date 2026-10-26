namespace RentFlow.Application.Bookings.Dtos;

public record BookingPenaltyDto(
    Guid Id,
    BookingPenaltyType Type,
    decimal Amount
);