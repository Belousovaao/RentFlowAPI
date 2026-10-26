using RentFlow.Domain.Auth;

namespace RentFlow.Application.Auth.Otp;


public sealed record OtpAuthenticationResult(
    OtpVerificationResult VerificationResult,
    OtpAuthenticationStatus? Status,
    AppUser? User,
    string? RegistrationTicket);

public enum OtpAuthenticationStatus
{
    Authenticated = 0,
    RegistrationRequired = 1
}