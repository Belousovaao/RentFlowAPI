namespace RentFlow.Application.Auth.Otp;

public sealed record OtpRateLimitDecision(
    bool Allowed,
    OtpRateLimitResult Reason,
    int RetryAfter);