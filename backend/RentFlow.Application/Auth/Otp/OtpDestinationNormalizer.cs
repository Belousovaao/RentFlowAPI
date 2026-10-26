namespace RentFlow.Application.Auth.Otp;

public sealed class OtpDestinationNormalizer : IOtpDestinationNormalizer
{
    public string Normalize(
        OtpChannel channel,
        string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        return channel switch
        {
            OtpChannel.Email => NormalizeEmail(destination),
            OtpChannel.Phone => NormalizePhone(destination),

            _ => throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                "Unsupported OTP channel.")
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        if (digits.Length == 0)
        {
            throw new ArgumentException(
                "Phone number is invalid.",
                nameof(phone));
        }

        return $"+{digits}";
    }
}