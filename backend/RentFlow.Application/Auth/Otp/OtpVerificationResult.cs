namespace RentFlow.Application.Auth.Otp;

public enum OtpVerificationResult
{
    Success = 0,
    NotFound = 1,
    InvalidCode = 2,
    TooManyAttempts = 3
}