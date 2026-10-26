using Microsoft.Extensions.Logging;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Redis;

public sealed class DevelopmentEmailOtpSender : IEmailOtpSender
{
    private readonly ILogger<DevelopmentEmailOtpSender> _logger;

    public DevelopmentEmailOtpSender(
        ILogger<DevelopmentEmailOtpSender> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public Task SendAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Development OTP email sent to {Email}. Code: {Code}",
            email,
            code);

        return Task.CompletedTask;
    }
}