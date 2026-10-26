using FluentAssertions;
using RentFlow.Application.Auth.Otp;

namespace RentFlow.Test.UnitTests;

public sealed class OtpDestinationNormalizerTests
{
    private readonly OtpDestinationNormalizer _normalizer = new();

    [Theory]
    [InlineData("TEST@EXAMPLE.COM", "test@example.com")]
    [InlineData(" Test@Example.Com ", "test@example.com")]
    [InlineData("user@example.com", "user@example.com")]
    public void Normalize_Email_ReturnsCanonicalEmail(
        string email,
        string expected)
    {
        var result = _normalizer.Normalize(
            OtpChannel.Email,
            email);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(
        " +7 (900) 123-45-67 ",
        "+79001234567")]
    [InlineData(
        "+79001234567",
        "+79001234567")]
    [InlineData(
        "7 900 123 45 67",
        "+79001234567")]
    public void Normalize_Phone_ReturnsCanonicalPhone(
        string phone,
        string expected)
    {
        var result = _normalizer.Normalize(
            OtpChannel.Phone,
            phone);

        result.Should().Be(expected);
    }

    [Fact]
    public void Normalize_PhoneWithoutDigits_Throws()
    {
        var act = () => _normalizer.Normalize(
            OtpChannel.Phone,
            "abc");

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Phone number is invalid.*");
    }

    [Fact]
    public void Normalize_EmptyDestination_Throws()
    {
        var act = () => _normalizer.Normalize(
            OtpChannel.Email,
            " ");

        act.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Normalize_UnsupportedChannel_Throws()
    {
        var act = () => _normalizer.Normalize(
            (OtpChannel)999,
            "test@example.com");

        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("Unsupported OTP channel.*");
    }
}