namespace RentFlow.Application.Interfaces;

public interface IPhoneOtpSender
{
    Task SendAsync(
        string phone,
        string code,
        CancellationToken cancellationToken = default);
}