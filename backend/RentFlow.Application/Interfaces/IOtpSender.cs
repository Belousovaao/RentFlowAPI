using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Interfaces;

public interface IOtpSender
{
    Task SendAsync(
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default);
}