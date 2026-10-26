namespace RentFlow.Application.Interfaces;

public interface IEmailOtpSender
{
    Task SendAsync(
        string email,
        string code,
        CancellationToken cancellationToken = default);
}