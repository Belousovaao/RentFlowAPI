using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Auth.Registration;

namespace RentFlow.Application.Interfaces;

public interface IRegistrationTicketService
{
    Task<string> CreateAsync(
        OtpChannel channel,
        string destinationHash,
        CancellationToken cancellationToken = default);

    Task<RegistrationTicket?> GetAsync(
        string ticket,
        CancellationToken cancellationToken = default);

    Task<RegistrationTicket?> ConsumeAsync(
        string ticket,
        CancellationToken cancellationToken = default);
}