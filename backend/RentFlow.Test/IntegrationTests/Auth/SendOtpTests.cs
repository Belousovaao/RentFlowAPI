using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Api.Services.Responses;
using RentFlow.Application.Interfaces;
using RentFlow.Test.IntegrationTests.Infrastructure.Redis;
using Xunit;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class SendOtpTests : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _redisFixture;
    public SendOtpTests(RedisContainerFixture redisFixture)
    {
        _redisFixture = redisFixture;
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Redis:ConnectionString"] =
                                _redisFixture.ConnectionString,

                            ["OtpSecurity:HmacKey"] =
                                Convert.ToBase64String(
                                    new byte[32])
                        });
                });
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<TestEmailOtpSender>();
                    services.AddSingleton<IEmailOtpSender>(sp => sp.GetRequiredService<TestEmailOtpSender>());
                    services.AddSingleton<IStartupFilter>(new TestRemoteIpStartupFilter());
                });
            });
    }

    private sealed class TestRemoteIpStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(async (context, nextMiddleware) =>
                {
                    context.Connection.RemoteIpAddress = IPAddress.Loopback;

                    await nextMiddleware();
                });

                next(app);
            };
        }
    }


    [Fact]
    public async Task SendOtp_WithEmail_ReturnsSuccess()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var destination = $"test-{Guid.NewGuid():N}@example.com";
        var request = new
        {
            channel = "email",
            destination
        };
        var response = await client.PostAsJsonAsync("/api/auth/otp/send", request);
        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        sender.LastCode.Should().NotBeNullOrWhiteSpace();
        sender.LastEmail.Should().Be(destination);
        var body = await response.Content.ReadFromJsonAsync<SendOtpResponse>();
        body.Should().NotBeNull();
        body!.ChallengeId.Should().NotBeEmpty();
        body.ExpiresIn.Should().Be(300);
        body.RetryAfter.Should().Be(60);
    }

    [Fact]
    public async Task SendOtp_WhenResendCooldownIsActive_ReturnsTooManyRequests()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var destination = $"test-{Guid.NewGuid():N}@example.com";
        var request = new
        {
            channel = "email",
            destination
        };
        var firstResponse = await client.PostAsJsonAsync("/api/auth/otp/send", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await firstResponse.Content.ReadAsStringAsync();

        if (!firstResponse.IsSuccessStatusCode)
        {
            throw new Exception(
                $"OTP request failed. " +
                $"Status: {(int)firstResponse.StatusCode} {firstResponse.StatusCode}. " +
                $"Body: {content}");
        }

        var secondResponse = await client.PostAsJsonAsync("/api/auth/otp/send", request);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        secondResponse.Headers.RetryAfter.Should().NotBeNull();
        secondResponse.Headers.RetryAfter!.Delta.Should().NotBeNull();
        secondResponse.Headers.RetryAfter!.Delta!.Value.Should().BeGreaterThan(TimeSpan.Zero);

        var body = await secondResponse.Content.ReadFromJsonAsync<RateLimitResponse>();
        body.Should().NotBeNull();
        body!.RetryAfter.Should().BeGreaterThan(0);
    }

    private sealed record RateLimitResponse(
        string Message,
        int RetryAfter);
}