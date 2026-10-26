using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Redis;

public sealed class OtpSender : IOtpSender
{
    private readonly IEmailOtpSender _emailSender;
    private readonly IPhoneOtpSender _phoneSender;

    public OtpSender(
        IEmailOtpSender emailSender,
        IPhoneOtpSender phoneSender)
    {
        ArgumentNullException.ThrowIfNull(emailSender);
        ArgumentNullException.ThrowIfNull(phoneSender);

        _emailSender = emailSender;
        _phoneSender = phoneSender;
    }

    public Task SendAsync(
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return channel switch
        {
            OtpChannel.Email =>
                _emailSender.SendAsync(
                    destination,
                    code,
                    cancellationToken),

            OtpChannel.Phone =>
                _phoneSender.SendAsync(
                    destination,
                    code,
                    cancellationToken),

            _ => throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                "Unsupported OTP channel.")
        };
    }
}