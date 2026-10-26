namespace RentFlow.Api.Services.Requests;

public record ChangeBookingPeriodRequest(
    DateOnly StartDate,
    DateOnly EndDate
);
