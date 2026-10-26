using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Api.Services.Responses;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;
using RentFlow.Test.IntegrationTests.Infrastructure.Redis;
using Xunit;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class VerifyOtpTests : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _redisFixture;

    public VerifyOtpTests(RedisContainerFixture redisFixture)
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
                    configuration.AddInMemoryCollection(
                        new Dictionary<string, string?>
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

                    services.AddSingleton<IEmailOtpSender>(
                        sp => sp.GetRequiredService<TestEmailOtpSender>());

                    services.AddSingleton<TestAppUserRepository>();

                    services.AddSingleton<IAppUserRepository>(
                        sp => sp.GetRequiredService<TestAppUserRepository>());

                    services.AddSingleton<IStartupFilter>(
                        new TestRemoteIpStartupFilter());
                });
            });
    }

    private static async Task<VerifyOtpResponse?> ReadVerifyOtpResponseAsync(HttpResponseMessage response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        options.Converters.Add(new JsonStringEnumConverter());

        return await response.Content.ReadFromJsonAsync<VerifyOtpResponse>(
            options);
    }
    private sealed class TestRemoteIpStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(async (context, nextMiddleware) =>
                {
                    context.Connection.RemoteIpAddress =
                        IPAddress.Loopback;

                    await nextMiddleware();
                });

                next(app);
            };
        }
    }

    [Fact]
    public async Task VerifyOtp_WithValidCode_ReturnsSuccess()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination = $"verify-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();
        sendBody!.ChallengeId.Should().NotBeEmpty();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastEmail.Should().Be(destination);
        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody.ChallengeId,
                channel = "email",
                destination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var verifyBody = await ReadVerifyOtpResponseAsync(verifyResponse);

        verifyBody.Should().NotBeNull();
        verifyBody!.Verified.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidCode_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"invalid-code-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination,
                code = "000000"
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyOtp_WithWrongDestination_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"destination-{Guid.NewGuid():N}@example.com";

        var wrongDestination =
            $"wrong-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination = wrongDestination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyOtp_WithWrongDestination_DoesNotConsumeChallenge()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"destination-reuse-{Guid.NewGuid():N}@example.com";

        var wrongDestination =
            $"wrong-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var wrongDestinationResponse =
            await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                new
                {
                    challengeId = sendBody!.ChallengeId,
                    channel = "email",
                    destination = wrongDestination,
                    code = sender.LastCode
                });

        wrongDestinationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);

        var correctDestinationResponse =
            await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                new
                {
                    challengeId = sendBody.ChallengeId,
                    channel = "email",
                    destination,
                    code = sender.LastCode
                });

        correctDestinationResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyOtp_AfterSuccessfulVerification_ReturnsBadRequest()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"verify-once-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var request = new
        {
            challengeId = sendBody!.ChallengeId,
            channel = "email",
            destination,
            code = sender.LastCode
        };

        var firstResponse =
            await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                request);

        firstResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var secondResponse =
            await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                request);

        secondResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task VerifyOtp_WithFiveInvalidCodes_ReturnsTooManyRequests()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"attempts-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        for (var attempt = 1; attempt <= 4; attempt++)
        {
            var response = await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                new
                {
                    challengeId = sendBody!.ChallengeId,
                    channel = "email",
                    destination,
                    code = "000000"
                });

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.BadRequest);
        }

        var fifthResponse =
            await client.PostAsJsonAsync(
                "/api/auth/otp/verify",
                new
                {
                    challengeId = sendBody!.ChallengeId,
                    channel = "email",
                    destination,
                    code = "000000"
                });

        fifthResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task VerifyOtp_WithDifferentEmailCasing_ReturnsSuccess()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"verify-case-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination = destination.ToUpperInvariant()
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyOtp_WithExistingUser_ReturnsAuthenticatedWithToken()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"existing-user-{Guid.NewGuid():N}@example.com";

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = destination,
            FirstName = "Test",
            LastName = "User"
        };

        var users =
            factory.Services
                .GetRequiredService<TestAppUserRepository>();

        users.Add(user);

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var verifyBody = await ReadVerifyOtpResponseAsync(verifyResponse);

        verifyBody.Should().NotBeNull();

        verifyBody!.Verified
            .Should()
            .BeTrue();

        verifyBody.Status
            .Should()
            .Be(OtpAuthenticationStatus.Authenticated);

        verifyBody.Token
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task VerifyOtp_WithUnknownUser_ReturnsRegistrationRequiredWithoutToken()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination = $"new-user-{Guid.NewGuid():N}@example.com";

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody = await sendResponse.Content.ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender = factory.Services.GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var verifyBody = await ReadVerifyOtpResponseAsync(verifyResponse);

        verifyBody.Should().NotBeNull();

        verifyBody!.Verified
            .Should()
            .BeTrue();

        verifyBody.Status
            .Should()
            .Be(OtpAuthenticationStatus.RegistrationRequired);

        verifyBody.Token
            .Should()
            .BeNull();

        verifyBody.RegistrationTicket
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task VerifyOtp_WithExistingUser_ReturnsTokenForCorrectUser()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var destination =
            $"jwt-user-{Guid.NewGuid():N}@example.com";

        var userId = Guid.NewGuid();

        var user = new AppUser
        {
            Id = userId,
            Email = destination,
            FirstName = "Test",
            LastName = "User"
        };

        var users = factory.Services
            .GetRequiredService<TestAppUserRepository>();

        users.Add(user);

        var sendResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/send",
            new
            {
                channel = "email",
                destination
            });

        sendResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var sendBody =
            await sendResponse.Content
                .ReadFromJsonAsync<SendOtpResponse>();

        sendBody.Should().NotBeNull();

        var sender =
            factory.Services
                .GetRequiredService<TestEmailOtpSender>();

        sender.LastCode.Should().NotBeNullOrWhiteSpace();

        var verifyResponse = await client.PostAsJsonAsync(
            "/api/auth/otp/verify",
            new
            {
                challengeId = sendBody!.ChallengeId,
                channel = "email",
                destination,
                code = sender.LastCode
            });

        verifyResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var verifyBody = await ReadVerifyOtpResponseAsync(verifyResponse);

        verifyBody.Should().NotBeNull();
        verifyBody!.Token.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(
            verifyBody.Token);

        var nameIdentifier =
            token.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.NameIdentifier);

        nameIdentifier.Should().NotBeNull();

        nameIdentifier!.Value
            .Should()
            .Be(userId.ToString());
    }

    
}