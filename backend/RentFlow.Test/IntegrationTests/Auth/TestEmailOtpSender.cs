using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class TestEmailOtpSender : IEmailOtpSender
{
    private readonly object _sync = new();
    public string? LastEmail { get; private set; }
    public string? LastCode { get; private set; }

    public Task SendAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            LastEmail = email;
            LastCode = code;
        }

        return Task.CompletedTask;
    }

    public void Reset()
    {
        lock (_sync)
        {
            LastEmail = null;
            LastCode = null;
        }
    }
}