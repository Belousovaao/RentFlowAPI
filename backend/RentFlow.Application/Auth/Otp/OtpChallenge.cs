namespace RentFlow.Application.Auth.Otp;

public sealed class OtpChallenge
{
    public Guid ChallengeId { get; init; }
    public required string Channel { get; init; }
    public required string DestinationHash { get; init; }
    public required string CodeHash { get; init; }
    public int Attempts { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}