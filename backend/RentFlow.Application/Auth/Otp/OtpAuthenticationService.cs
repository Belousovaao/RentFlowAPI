using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;

namespace RentFlow.Application.Auth.Otp;

public sealed class OtpAuthenticationService : IOtpAuthenticationService
{
    private readonly IOtpService _otpService;
    private readonly IAppUserRepository _users;
    private readonly IOtpDestinationNormalizer _destinationNormalizer;
    private readonly IRegistrationTicketService _registrationTickets;
    private readonly IOtpSecurity _security;

    public OtpAuthenticationService(
        IOtpService otpService,
        IAppUserRepository users,
        IOtpDestinationNormalizer otpDestinationNormalizer,
        IRegistrationTicketService registrationTickets,
        IOtpSecurity security)
    {
        ArgumentNullException.ThrowIfNull(otpService);
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(otpDestinationNormalizer);
        ArgumentNullException.ThrowIfNull(registrationTickets);
        ArgumentNullException.ThrowIfNull(security);

        _otpService = otpService;
        _users = users;
        _destinationNormalizer = otpDestinationNormalizer;
        _registrationTickets = registrationTickets;
        _security = security;
    }

    public async Task<OtpAuthenticationResult> VerifyAsync(
        Guid challengeId,
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default)
    {
        var verificationResult = await _otpService.VerifyAsync(
            challengeId,
            channel,
            destination,
            code,
            cancellationToken);

        if (verificationResult != OtpVerificationResult.Success)
        {
            return new OtpAuthenticationResult(
                verificationResult,
                null,
                null,
                null);
        }

        var normalizedDestination = _destinationNormalizer.Normalize(
            channel,
            destination);

        var destinationHash = _security.HashDestination(normalizedDestination);

        AppUser? user = channel switch
        {
            OtpChannel.Email => await _users.GetByEmailAsync(
                normalizedDestination,
                cancellationToken),

            OtpChannel.Phone => await _users.GetByPhoneAsync(
                normalizedDestination,
                cancellationToken),

            _ => throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                "Unsupported OTP channel.")
        };

        if (user is not null)
        {
            return new OtpAuthenticationResult(
                verificationResult,
                OtpAuthenticationStatus.Authenticated,
                user,
                null);
        }

        var registrationTicket = await _registrationTickets.CreateAsync(
            channel,
            destinationHash,
            cancellationToken);

        return new OtpAuthenticationResult(
                verificationResult,
                OtpAuthenticationStatus.RegistrationRequired,
                null,
                registrationTicket);
    }
}