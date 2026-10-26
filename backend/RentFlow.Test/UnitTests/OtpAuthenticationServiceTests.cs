using FluentAssertions;
using Moq;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;

namespace RentFlow.Test.UnitTests;

public sealed class OtpAuthenticationServiceTests
{
    private readonly Mock<IOtpService> _otpService = new();
    private readonly Mock<IAppUserRepository> _users = new();
    private readonly Mock<IOtpDestinationNormalizer> _destinationNormalizer = new();
    private readonly Mock<IOtpSecurity> _security = new();
    private readonly Mock<IRegistrationTicketService> _registrationTickets = new();


    private OtpAuthenticationService CreateService()
    {
        return new OtpAuthenticationService(
            _otpService.Object,
            _users.Object,
            _destinationNormalizer.Object,
            _registrationTickets.Object,
            _security.Object);
    }

    [Fact]
    public async Task VerifyAsync_WithExistingEmailUser_ReturnsAuthenticated()
    {
        var challengeId = Guid.NewGuid();

        const string destination = "TEST@EXAMPLE.COM";
        const string normalizedDestination = "test@example.com";
        const string code = "483921";

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedDestination,
            FirstName = "Test",
            LastName = "User"
        };

        _otpService
            .Setup(x => x.VerifyAsync(
                challengeId,
                OtpChannel.Email,
                destination,
                code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OtpVerificationResult.Success);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(normalizedDestination);

        _users
            .Setup(x => x.GetByEmailAsync(
                normalizedDestination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Email,
            destination,
            code);

        result.VerificationResult
            .Should()
            .Be(OtpVerificationResult.Success);

        result.Status
            .Should()
            .Be(OtpAuthenticationStatus.Authenticated);

        result.User
            .Should()
            .BeSameAs(user);

        result.RegistrationTicket
            .Should()
            .BeNull();

        _registrationTickets.Verify(
            x => x.CreateAsync(
                It.IsAny<OtpChannel>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task VerifyAsync_WithExistingPhoneUser_ReturnsAuthenticated()
    {
        var challengeId = Guid.NewGuid();

        const string destination = " +7 (900) 123-45-67 ";
        const string normalizedDestination = "+79001234567";
        const string code = "483921";

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Phone = normalizedDestination,
            FirstName = "Test",
            LastName = "User"
        };

        _otpService
            .Setup(x => x.VerifyAsync(
                challengeId,
                OtpChannel.Phone,
                destination,
                code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OtpVerificationResult.Success);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Phone,
                destination))
            .Returns(normalizedDestination);

        _users
            .Setup(x => x.GetByPhoneAsync(
                normalizedDestination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Phone,
            destination,
            code);

        result.VerificationResult
            .Should()
            .Be(OtpVerificationResult.Success);

        result.Status
            .Should()
            .Be(OtpAuthenticationStatus.Authenticated);

        result.User
            .Should()
            .BeSameAs(user);
        
        result.RegistrationTicket
            .Should()
            .BeNull();

        _registrationTickets.Verify(
            x => x.CreateAsync(
                It.IsAny<OtpChannel>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task VerifyAsync_WithUnknownEmail_ReturnsRegistrationRequired()
    {
        var challengeId = Guid.NewGuid();

        const string destination = "test@example.com";
        const string code = "483921";
        const string destinationHash = "destination-hash";
        const string registrationTicket = "registration-ticket";

        _otpService
            .Setup(x => x.VerifyAsync(
                challengeId,
                OtpChannel.Email,
                destination,
                code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OtpVerificationResult.Success);

        _destinationNormalizer
            .Setup(x => x.Normalize(
                OtpChannel.Email,
                destination))
            .Returns(destination);

        _security
            .Setup(x => x.HashDestination(destination))
            .Returns(destinationHash);

        _users
            .Setup(x => x.GetByEmailAsync(
                destination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);

        _registrationTickets
            .Setup(x => x.CreateAsync(
                OtpChannel.Email,
                destinationHash,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(registrationTicket);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Email,
            destination,
            code);

        result.VerificationResult
            .Should()
            .Be(OtpVerificationResult.Success);

        result.Status
            .Should()
            .Be(OtpAuthenticationStatus.RegistrationRequired);

        result.User
            .Should()
            .BeNull();

        result.RegistrationTicket
            .Should()
            .Be(registrationTicket);

        _security.Verify(
            x => x.HashDestination(destination),
            Times.Once);

        _registrationTickets.Verify(
            x => x.CreateAsync(
                OtpChannel.Email,
                destinationHash,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(OtpVerificationResult.InvalidCode)]
    [InlineData(OtpVerificationResult.NotFound)]
    [InlineData(OtpVerificationResult.TooManyAttempts)]
    public async Task VerifyAsync_WhenOtpVerificationFails_DoesNotLookupUser(OtpVerificationResult verificationResult)
    {
        var challengeId = Guid.NewGuid();

        const string destination = "test@example.com";
        const string code = "483921";

        _otpService
            .Setup(x => x.VerifyAsync(
                challengeId,
                OtpChannel.Email,
                destination,
                code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verificationResult);

        var service = CreateService();

        var result = await service.VerifyAsync(
            challengeId,
            OtpChannel.Email,
            destination,
            code);

        result.VerificationResult
            .Should()
            .Be(verificationResult);

        result.Status
            .Should()
            .BeNull();

        result.User
            .Should()
            .BeNull();

        result.RegistrationTicket
            .Should()
            .BeNull();

        _destinationNormalizer.Verify(
            x => x.Normalize(
                It.IsAny<OtpChannel>(),
                It.IsAny<string>()),
            Times.Never);

        _users.Verify(
            x => x.GetByEmailAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _users.Verify(
            x => x.GetByPhoneAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _registrationTickets.Verify(
            x => x.CreateAsync(
                It.IsAny<OtpChannel>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}