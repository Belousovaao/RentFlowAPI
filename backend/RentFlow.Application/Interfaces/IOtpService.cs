using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Interfaces;

public interface IOtpService
{
    Task<OtpVerificationResult> VerifyAsync(
        Guid challengeId,
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default);

    Task<OtpSendResult> SendAsync(
        OtpChannel channel,
        string destination,
        string ipAddress,
        CancellationToken cancellationToken = default);
}