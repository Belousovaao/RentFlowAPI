namespace RentFlow.Application.Bookings.Dtos;
public record BusyPeriodDto(
    DateTime StartDate,
    DateTime EndDate
);