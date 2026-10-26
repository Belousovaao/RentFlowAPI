using RentFlow.Application.Interfaces;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class TestPhoneOtpSender : IPhoneOtpSender
{
    private readonly object _sync = new();
    public string? LastPhone { get; private set; }
    public string? LastCode { get; private set; }

    public Task SendAsync(
        string phone,
        string code,
        CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            LastPhone = phone;
            LastCode = code;
        }

        return Task.CompletedTask;
    }

    public void Reset()
    {
        lock (_sync)
        {
            LastPhone = null;
            LastCode = null;
        }
    }
}