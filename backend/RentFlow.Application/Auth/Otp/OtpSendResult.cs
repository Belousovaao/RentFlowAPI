namespace RentFlow.Application.Auth.Otp;

public sealed record OtpSendResult(
    OtpSendResultStatus Status,
    Guid? ChallengeId,
    int ExpiresIn,
    int RetryAfter,
    OtpRateLimitResult? RateLimitResult);
public enum OtpSendResultStatus
{
    Sent = 0,
    RateLimited = 1
}