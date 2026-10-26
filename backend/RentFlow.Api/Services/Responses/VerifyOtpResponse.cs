using RentFlow.Application.Auth.Otp;

namespace RentFlow.Api.Services.Responses;

public sealed record VerifyOtpResponse(
    bool Verified,
    OtpAuthenticationStatus Status,
    string? Token,
    string? RegistrationTicket);