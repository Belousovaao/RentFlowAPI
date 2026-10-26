using Microsoft.Extensions.Logging;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Redis;

public sealed class DevelopmentPhoneOtpSender : IPhoneOtpSender
{
    private readonly ILogger<DevelopmentPhoneOtpSender> _logger;

    public DevelopmentPhoneOtpSender(ILogger<DevelopmentPhoneOtpSender> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public Task SendAsync(
        string phone,
        string code,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Development OTP SMS sent to {Phone}. Code: {Code}",
            phone,
            code);

        return Task.CompletedTask;
    }
}