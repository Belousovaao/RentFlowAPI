namespace RentFlow.Api.Services.Requests;

public sealed record CancelBookingRequest(string? Reason = null);