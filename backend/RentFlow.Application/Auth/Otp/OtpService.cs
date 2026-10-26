using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Auth.Otp;

public sealed class OtpService : IOtpService
{
    private readonly IOtpSecurity _security;
    private readonly IOtpRateLimiter _rateLimiter;
    private readonly IOtpStore _store;
    private readonly IOtpSender _sender;
    private readonly IOtpDestinationNormalizer _destinationNormalizer;
    private readonly OtpOptions _options;

    public OtpService(
        IOtpSecurity security,
        IOtpRateLimiter rateLimiter,
        IOtpStore store,
        IOtpSender sender,
        OtpOptions options,
        IOtpDestinationNormalizer destinationNormalizer)
    {
        ArgumentNullException.ThrowIfNull(security);
        ArgumentNullException.ThrowIfNull(rateLimiter);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(destinationNormalizer);
        ArgumentNullException.ThrowIfNull(options);

        _security = security;
        _rateLimiter = rateLimiter;
        _store = store;
        _sender = sender;
        _options = options;
        _destinationNormalizer = destinationNormalizer;

        ValidateOptions(_options);
    }

    public async Task<OtpSendResult> SendAsync(
        OtpChannel channel,
        string destination,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var normalizedDestination = _destinationNormalizer.Normalize(channel, destination);

        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);

        var destinationHash = _security.HashDestination(normalizedDestination);

        var ipHash = _security.HashIp(ipAddress);

        var ipDestinationHash = _security.HashIpDestination(ipAddress, normalizedDestination);

        var rateLimitDecision = await _rateLimiter.CheckAndConsumeAsync(
            destinationHash,
            ipHash,
            ipDestinationHash,
            cancellationToken);

        if (!rateLimitDecision.Allowed)
        {
            return new OtpSendResult(
                OtpSendResultStatus.RateLimited,
                null,
                0,
                rateLimitDecision.RetryAfter,
                rateLimitDecision.Reason);
        }

        var code = _security.GenerateCode();

        if (code.Length != _options.CodeLength)
        {
            throw new InvalidOperationException(
                $"OTP generator returned code with invalid length. " +
                $"Expected {_options.CodeLength}, actual {code.Length}.");
        }

        var challengeId = Guid.NewGuid();
        var challenge = new OtpChallenge
        {
            ChallengeId = challengeId,
            Channel = channel.ToString().ToLowerInvariant(),
            DestinationHash = destinationHash,
            CodeHash = _security.HashCode(code),
            Attempts = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var challengeTtl =
            TimeSpan.FromSeconds(
                _options.ChallengeTtlSeconds);

        await _store.CreateAsync(
            challenge,
            challengeTtl,
            cancellationToken);

        try
        {
            await _sender.SendAsync(
                channel,
                normalizedDestination,
                code,
                cancellationToken);
        }
        catch
        {
            await _store.DeleteAsync(
                challengeId,
                CancellationToken.None);

            throw;
        }

        return new OtpSendResult(
            OtpSendResultStatus.Sent,
            challengeId,
            _options.ChallengeTtlSeconds,
            _options.ResendCooldownSeconds,
            null);
    }

    public async Task<OtpVerificationResult> VerifyAsync(
        Guid challengeId,
        OtpChannel channel,
        string destination,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (challengeId == Guid.Empty)
        {
            throw new ArgumentException("Challenge ID cannot be empty.", nameof(challengeId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        
        if (!IsValidCodeFormat(code))
        {
            return OtpVerificationResult.InvalidCode;
        }
        var normalizedDestination = _destinationNormalizer.Normalize(
            channel,
            destination);

        var destinationHash = _security.HashDestination(normalizedDestination);
        var codeHash = _security.HashCode(code);

        return await _store.VerifyAsync(
            challengeId,
            destinationHash,
            codeHash,
            _options.MaxAttempts,
            cancellationToken);
    }

    private bool IsValidCodeFormat(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        if (code.Length != _options.CodeLength)
        {
            return false;
        }

        foreach (var character in code)
        {
            if (character is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static void ValidateOptions(OtpOptions options)
    {
        if (options.CodeLength <= 0)
        {
            throw new InvalidOperationException("Otp:CodeLength must be greater than zero.");
        }

        if (options.MaxAttempts <= 0)
        {
            throw new InvalidOperationException("Otp:MaxAttempts must be greater than zero.");
        }

        if (options.ChallengeTtlSeconds <= 0)
        {
            throw new InvalidOperationException("Otp:ChallengeTtlSeconds must be greater than zero.");
        }

        if (options.ResendCooldownSeconds <= 0)
        {
            throw new InvalidOperationException("Otp:ResendCooldownSeconds must be greater than zero.");
        }
    }
}