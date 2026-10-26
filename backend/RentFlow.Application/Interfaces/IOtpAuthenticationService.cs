using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Interfaces;

public interface IOtpAuthenticationService
{
    Task<OtpAuthenticationResult> VerifyAsync(
        Guid challengeId,
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default);
}