using FluentAssertions;
using Moq;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;

namespace RentFlow.Test.UnitTests;

public sealed class OtpServiceTests
{
    private readonly Mock<IOtpSecurity> _security = new();
    private readonly Mock<IOtpRateLimiter> _rateLimiter = new();
    private readonly Mock<IOtpStore> _store = new();
    private readonly Mock<IOtpSender> _sender = new();
    private readonly Mock<IOtpDestinationNormalizer> _destinationNormalizer = new();

    private readonly OtpOptions _options = new()
    {
        CodeLength = 6,
        MaxAttempts = 5,
        ChallengeTtlSeconds = 300,
        ResendCooldownSeconds = 60
    };

    private OtpService CreateService()
    {
        return new OtpService(
            _security.Object,
            _rateLimiter.Object,
            _store.Object,
            _sender.Object,
            _options,
            _destinationNormalizer.Object);
    }
    
    [Fact]
    public async Task SendAsync_WhenRateLimitAllows_ReturnsSentResult()
    {
        const string destination = "TEST@EXAMPLE.COM";
        const string normalizedDestination = "test@example.com";
        const string ipAddress = "127.0.0.1";
        const string destinationHash = "destination-hash";
        const string ipHash = "ip-hash";
        const string ipDestinationHash = "ip-destination-hash";
        const string code = "483921";
        const string codeHash = "code-hash";

        _security
            .Setup(x => x.HashDestination(normalizedDestination))
            .Returns(destinationHash);

        _security
            .Setup(x => x.HashIp(ipAddress))
            .Returns(ipHash);

        _security
            .Setup(x => x.HashIpDestination(
                ipAddress,
                normalizedDestination))
            .Returns(ipDestinationHash);

        _rateLimiter
            .Setup(x => x.CheckAndConsumeAsync(
                destinationHash,
                ipHash,
                ipDestinationHash,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OtpRateLimitDecision(
                    true,
                    OtpRateLimitResult.Allowed,
                    0));

        _security
            .Setup(x => x.GenerateCode())
            .Returns(code);

        _security
            .Setup(x => x.HashCode(code))
            .Returns(codeHash);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(normalizedDestination);

        var service = CreateService();

        var result = await service.SendAsync(
            OtpChannel.Email,
            destination,
            ipAddress);

        result.Status
            .Should()
            .Be(OtpSendResultStatus.Sent);

        result.ChallengeId
            .Should()
            .NotBeNull();

        result.ExpiresIn
            .Should()
            .Be(300);

        result.RetryAfter
            .Should()
            .Be(60);

        result.RateLimitResult
            .Should()
            .BeNull();

        _store.Verify(
            x => x.CreateAsync(
                It.Is<OtpChallenge>(challenge =>
                    challenge.ChallengeId == result.ChallengeId &&
                    challenge.Channel == "email" &&
                    challenge.DestinationHash == destinationHash &&
                    challenge.CodeHash == codeHash &&
                    challenge.Attempts == 0),
                TimeSpan.FromSeconds(300),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _sender.Verify(
            x => x.SendAsync(
                OtpChannel.Email,
                normalizedDestination,
                code,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WhenRateLimitExceeded_DoesNotCreateChallengeOrSendCode()
    {
        const string destination = "test@example.com";
        const string ipAddress = "127.0.0.1";

        _security
            .Setup(x => x.HashDestination(destination))
            .Returns("destination-hash");

        _security
            .Setup(x => x.HashIp(ipAddress))
            .Returns("ip-hash");

        _security
            .Setup(x => x.HashIpDestination(
                ipAddress,
                destination))
            .Returns("ip-destination-hash");

        _rateLimiter
            .Setup(x => x.CheckAndConsumeAsync(
                "destination-hash",
                "ip-hash",
                "ip-destination-hash",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OtpRateLimitDecision(
                    false,
                    OtpRateLimitResult.ResendCooldownActive,
                    47));
        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(destination);
        var service = CreateService();

        var result = await service.SendAsync(
            OtpChannel.Email,
            destination,
            ipAddress);

        result.Status
            .Should()
            .Be(OtpSendResultStatus.RateLimited);

        result.ChallengeId
            .Should()
            .BeNull();

        result.ExpiresIn
            .Should()
            .Be(0);

        result.RetryAfter
            .Should()
            .Be(47);

        result.RateLimitResult
            .Should()
            .Be(
                OtpRateLimitResult.ResendCooldownActive);

        _store.Verify(
            x => x.CreateAsync(
                It.IsAny<OtpChallenge>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _sender.Verify(
            x => x.SendAsync(
                It.IsAny<OtpChannel>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _security.Verify(
            x => x.GenerateCode(),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_WhenSenderFails_DeletesChallengeAndRethrows()
    {
        const string destination = "test@example.com";
        const string ipAddress = "127.0.0.1";
        const string code = "483921";

        _security
            .Setup(x => x.HashDestination(destination))
            .Returns("destination-hash");

        _security
            .Setup(x => x.HashIp(ipAddress))
            .Returns("ip-hash");

        _security
            .Setup(x => x.HashIpDestination(
                ipAddress,
                destination))
            .Returns("ip-destination-hash");

        _rateLimiter
            .Setup(x => x.CheckAndConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OtpRateLimitDecision(
                    true,
                    OtpRateLimitResult.Allowed,
                    0));

        _security
            .Setup(x => x.GenerateCode())
            .Returns(code);

        _security
            .Setup(x => x.HashCode(code))
            .Returns("code-hash");

        var exception = new InvalidOperationException(
            "OTP provider failed.");

        _sender
            .Setup(x => x.SendAsync(
                OtpChannel.Email,
                destination,
                code,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(destination);

        var service = CreateService();

        var act = () => service.SendAsync(
            OtpChannel.Email,
            destination,
            ipAddress);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("OTP provider failed.");

        _store.Verify(
            x => x.DeleteAsync(
                It.IsAny<Guid>(),
                It.Is<CancellationToken>(
                    token => token == CancellationToken.None)),
            Times.Once);

        _rateLimiter.Verify(
            x => x.CheckAndConsumeAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task VerifyAsync_WithValidCode_DelegatesToStore()
    {
        var challengeId = Guid.NewGuid();
        const string destination = "TEST@EXAMPLE.COM";
        const string normalizedDestination = "test@example.com";
        const string destinationHash = "destination-hash";
        const string code = "483921";
        const string codeHash = "hashed-code";

        _security
            .Setup(x => x.HashDestination(normalizedDestination))
            .Returns(destinationHash);
        _security
            .Setup(x => x.HashCode(code))
            .Returns(codeHash);

        _store
            .Setup(x => x.VerifyAsync(
                challengeId,
                destinationHash,
                codeHash,
                5,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OtpVerificationResult.Success);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(normalizedDestination);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Email,
            destination,
            code);

        result.Should().Be(OtpVerificationResult.Success);

        _security.Verify(
            x => x.HashCode(code),
            Times.Once);

        _store.Verify(
            x => x.VerifyAsync(
                challengeId,
                destinationHash,
                codeHash,
                5,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12345a")]
    [InlineData("abcdef")]
    public async Task VerifyAsync_WithInvalidCodeFormat_ReturnsInvalidCode(string code)
    {
        var service = CreateService();

        var result = await service.VerifyAsync(
            Guid.NewGuid(),
            OtpChannel.Email,
            "test@example.com",
            code);

        result.Should().Be(OtpVerificationResult.InvalidCode);
        _security.Verify(
            x => x.HashDestination(It.IsAny<string>()),
            Times.Never);
        _security.Verify(
            x => x.HashCode(It.IsAny<string>()),
            Times.Never);

        _store.Verify(
            x => x.VerifyAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_WithPhone_NormalizesDestinationBeforeHashingAndSending()
    {
        const string phone = " +7 (900) 123-45-67 ";
        const string normalizedPhone = "+79001234567";
        const string ipAddress = "127.0.0.1";
        const string code = "483921";

        _security
            .Setup(x => x.HashDestination(normalizedPhone))
            .Returns("destination-hash");

        _security
            .Setup(x => x.HashIp(ipAddress))
            .Returns("ip-hash");

        _security
            .Setup(x => x.HashIpDestination(
                ipAddress,
                normalizedPhone))
            .Returns("ip-destination-hash");

        _rateLimiter
            .Setup(x => x.CheckAndConsumeAsync(
                "destination-hash",
                "ip-hash",
                "ip-destination-hash",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new OtpRateLimitDecision(
                    true,
                    OtpRateLimitResult.Allowed,
                    0));

        _security
            .Setup(x => x.GenerateCode())
            .Returns(code);

        _security
            .Setup(x => x.HashCode(code))
            .Returns("code-hash");

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                phone))
            .Returns(normalizedPhone);

        var service = CreateService();

        await service.SendAsync(
            OtpChannel.Phone,
            phone,
            ipAddress);

        _destinationNormalizer.Verify(
            x => x.Normalize(
                OtpChannel.Phone,
                phone),
            Times.Once);

        _security.Verify(
            x => x.HashDestination(normalizedPhone),
            Times.Once);

        _sender.Verify(
            x => x.SendAsync(
                OtpChannel.Phone,
                normalizedPhone,
                code,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task VerifyAsync_WithPhone_NormalizesDestinationBeforeHashing()
    {
        var challengeId = Guid.NewGuid();

        const string phone = " +7 (900) 123-45-67 ";
        const string normalizedPhone = "+79001234567";
        const string destinationHash = "destination-hash";

        const string code = "483921";
        const string codeHash = "code-hash";

        _security
            .Setup(x => x.HashDestination(normalizedPhone))
            .Returns(destinationHash);

        _security
            .Setup(x => x.HashCode(code))
            .Returns(codeHash);

        _store
            .Setup(x => x.VerifyAsync(
                challengeId,
                destinationHash,
                codeHash,
                5,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OtpVerificationResult.Success);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                phone))
            .Returns(normalizedPhone);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Phone,
            phone,
            code);

        result.Should()
            .Be(OtpVerificationResult.Success);

        _security.Verify(
            x => x.HashDestination(normalizedPhone),
            Times.Once);

        _store.Verify(
            x => x.VerifyAsync(
                challengeId,
                destinationHash,
                codeHash,
                5,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}