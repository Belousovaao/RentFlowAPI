namespace RentFlow.Infrastructure.Redis;

public sealed class OtpRateLimitOptions
{
    public const string SectionName = "OtpRateLimit";
    public int DestinationHourlyLimit { get; set; } = 5;
    public int DestinationDailyLimit { get; set; } = 10;
    public int IpTenMinuteLimit { get; set; } = 10;
    public int IpHourlyLimit { get; set; } = 30;
    public int IpDestinationHourlyLimit { get; set; } = 3;
}