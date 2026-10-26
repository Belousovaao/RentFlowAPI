using RentFlow.Domain.Auth;

namespace RentFlow.Application.Auth.Registration;

public sealed record RegistrationResult(
    RegistrationResultStatus Status,
    AppUser? User)
{
    public static RegistrationResult Success(AppUser user) =>
        new(RegistrationResultStatus.Success, user);

    public static RegistrationResult InvalidTicket() =>
        new(RegistrationResultStatus.InvalidTicket, null);

    public static RegistrationResult ContactMismatch() =>
        new(RegistrationResultStatus.ContactMismatch, null);

    public static RegistrationResult AlreadyExists() =>
        new(RegistrationResultStatus.AlreadyExists, null);

    public static RegistrationResult InvalidRequest() =>
        new(RegistrationResultStatus.InvalidRequest, null);
}