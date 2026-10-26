using RentFlow.Application.Auth.Otp;

namespace RentFlow.Application.Auth.Registration;

public sealed record RegistrationTicket(
    string Ticket,
    OtpChannel Channel,
    string DestinationHash);