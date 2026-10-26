using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Interfaces;

public interface IOtpRateLimiter
{
    Task<OtpRateLimitDecision> CheckAndConsumeAsync(
        string destinationHash,
        string ipHash,
        string ipDestinationHash,
        CancellationToken cancellationToken = default);
}