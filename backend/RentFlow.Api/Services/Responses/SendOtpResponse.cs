namespace RentFlow.Api.Services.Responses;

public sealed record SendOtpResponse(
    Guid ChallengeId,
    int ExpiresIn,
    int RetryAfter);