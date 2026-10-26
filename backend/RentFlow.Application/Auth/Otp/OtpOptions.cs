namespace RentFlow.Application.Auth.Otp;

public sealed class OtpOptions
{
    public const string SectionName = "Otp";

    public int CodeLength { get; set; } = 6;

    public int MaxAttempts { get; set; } = 5;

    public int ChallengeTtlSeconds { get; set; } = 300;

    public int ResendCooldownSeconds { get; set; } = 60;
}