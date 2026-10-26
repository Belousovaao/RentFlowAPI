using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Auth.Registration;
using RentFlow.Application.Interfaces;
using StackExchange.Redis;

namespace RentFlow.Infrastructure.Redis;

public sealed class RedisRegistrationTicketService: IRegistrationTicketService
{
    private const string KeyPrefix = "auth:registration:";

    private const string ConsumeScript = """
        local value = redis.call('GET', KEYS[1])

        if not value then
            return false
        end

        redis.call('DEL', KEYS[1])

        return value
        """;

    private readonly IConnectionMultiplexer _redis;
    private readonly RegistrationOptions _options;

    public RedisRegistrationTicketService(
        IConnectionMultiplexer redis,
        IOptions<RegistrationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(redis);
        ArgumentNullException.ThrowIfNull(options);

        _redis = redis;
        _options = options.Value;

        ValidateOptions(_options);
    }

    public async Task<string> CreateAsync(
        OtpChannel channel,
        string destinationHash,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationHash);

        if (!Enum.IsDefined(channel))
        {
            throw new ArgumentOutOfRangeException(
                nameof(channel),
                channel,
                "Unsupported OTP channel.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var ticket = GenerateTicket();

        var payload = new RegistrationTicketPayload(
            channel,
            destinationHash);

        var serializedPayload = JsonSerializer.Serialize(payload);

        var key = BuildKey(ticket);

        var database = _redis.GetDatabase();

        await database.StringSetAsync(
            key,
            serializedPayload,
            TimeSpan.FromSeconds(
                _options.TicketTtlSeconds));

        return ticket;
    }
    
    public async Task<RegistrationTicket?> GetAsync(
        string ticket,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticket);

        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(ticket);

        var database = _redis.GetDatabase();

        var value = await database.StringGetAsync(key);

        if (value.IsNull)
        {
            return null;
        }

        var payload = JsonSerializer.Deserialize<RegistrationTicketPayload>(
            (string)value!);

        if (payload is null)
        {
            throw new InvalidOperationException(
                "Registration ticket payload is invalid.");
        }

        return new RegistrationTicket(
            ticket,
            payload.Channel,
            payload.DestinationHash);
    }
        
    public async Task<RegistrationTicket?> ConsumeAsync(
        string ticket,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticket);

        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(ticket);

        var database = _redis.GetDatabase();

        var value = await database.ScriptEvaluateAsync(
            ConsumeScript,
            [new RedisKey(key)]);

        if (value.IsNull)
        {
            return null;
        }

        var payload = JsonSerializer.Deserialize<RegistrationTicketPayload>(
            (string)value!);

        if (payload is null)
        {
            throw new InvalidOperationException(
                "Registration ticket payload is invalid.");
        }

        return new RegistrationTicket(
            ticket,
            payload.Channel,
            payload.DestinationHash);
    }

    private static string GenerateTicket()
    {
        Span<byte> bytes = stackalloc byte[32];

        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string BuildKey(string ticket) =>
        $"{KeyPrefix}{ticket}";

    private static void ValidateOptions(
        RegistrationOptions options)
    {
        if (options.TicketTtlSeconds <= 0)
        {
            throw new InvalidOperationException(
                "Registration:TicketTtlSeconds must be greater than zero.");
        }
    }

    private sealed record RegistrationTicketPayload(
        OtpChannel Channel,
        string DestinationHash);
}