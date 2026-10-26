using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Interfaces;

public interface IOtpStore
{
    Task CreateAsync(
        OtpChallenge challenge,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);

    Task<OtpVerificationResult> VerifyAsync(
        Guid challengeId,
        string destinationHash,
        string codeHash,
        int maxAttempts,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid challengeId,
        CancellationToken cancellationToken = default);
}