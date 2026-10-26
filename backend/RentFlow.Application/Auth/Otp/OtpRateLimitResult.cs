namespace RentFlow.Application.Auth.Otp;

public enum OtpRateLimitResult
{
    Allowed = 0,
    DestinationHourlyLimitExceeded = 1,
    DestinationDailyLimitExceeded = 2,
    IpTenMinuteLimitExceeded = 3,
    IpHourlyLimitExceeded = 4,
    IpDestinationHourlyLimitExceeded = 5,
    ResendCooldownActive = 6
}