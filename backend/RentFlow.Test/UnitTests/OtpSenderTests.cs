using FluentAssertions;
using Moq;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using RentFlow.Infrastructure.Redis;

namespace RentFlow.Test.UnitTests;

public sealed class OtpSenderTests
{
    private readonly Mock<IEmailOtpSender> _emailSender = new();
    private readonly Mock<IPhoneOtpSender> _phoneSender = new();

    private OtpSender CreateSender()
    {
        return new OtpSender(
            _emailSender.Object,
            _phoneSender.Object);
    }

    [Fact]
    public async Task SendAsync_WithEmail_UsesEmailSender()
    {
        const string email = "test@example.com";
        const string code = "483921";

        var sender = CreateSender();

        await sender.SendAsync(
            OtpChannel.Email,
            email,
            code);

        _emailSender.Verify(
            x => x.SendAsync(
                email,
                code,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _phoneSender.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_WithPhone_UsesPhoneSender()
    {
        const string phone = "+79001234567";
        const string code = "483921";

        var sender = CreateSender();

        await sender.SendAsync(
            OtpChannel.Phone,
            phone,
            code);

        _phoneSender.Verify(
            x => x.SendAsync(
                phone,
                code,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _emailSender.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SendAsync_WithUnsupportedChannel_Throws()
    {
        var sender = CreateSender();

        var act = () => sender.SendAsync(
            (OtpChannel)999,
            "test@example.com",
            "483921");

        await act.Should()
            .ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task SendAsync_WithEmptyDestination_Throws()
    {
        var sender = CreateSender();

        var act = () => sender.SendAsync(
            OtpChannel.Email,
            "",
            "483921");

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendAsync_WithEmptyCode_Throws()
    {
        var sender = CreateSender();

        var act = () => sender.SendAsync(
            OtpChannel.Email,
            "test@example.com",
            "");

        await act.Should()
            .ThrowAsync<ArgumentException>();
    }
}