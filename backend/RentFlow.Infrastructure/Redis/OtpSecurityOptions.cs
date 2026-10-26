namespace RentFlow.Infrastructure.Redis;

public sealed class OtpSecurityOptions
{
    public const string SectionName = "OtpSecurity";
    public string HmacKey { get; set; } = string.Empty;
}