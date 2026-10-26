namespace RentFlow.Application.Auth.Registration;

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(
        string registrationTicket,
        RegisterCustomerData data,
        CancellationToken cancellationToken = default);
}