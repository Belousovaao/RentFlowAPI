namespace RentFlow.Application.Auth.Otp;

public interface IOtpDestinationNormalizer
{
    string Normalize(
        OtpChannel channel,
        string destination);
}