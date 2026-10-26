using Microsoft.Extensions.Options;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using StackExchange.Redis;

namespace RentFlow.Infrastructure.Redis;

public sealed class RedisOtpRateLimiter : IOtpRateLimiter
{
    private const string KeyPrefix = "ratelimit:otp:";
    private static readonly TimeSpan DestinationHourlyWindow = TimeSpan.FromHours(1);
    private static readonly TimeSpan DestinationDailyWindow = TimeSpan.FromDays(1);
    private static readonly TimeSpan IpTenMinuteWindow = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan IpHourlyWindow = TimeSpan.FromHours(1);
    private static readonly TimeSpan IpDestinationHourlyWindow = TimeSpan.FromHours(1);
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

    private const string RateLimitScript = """
        local destinationHourly = redis.call('GET', KEYS[1])
        local destinationDaily = redis.call('GET', KEYS[2])
        local ipTenMinute = redis.call('GET', KEYS[3])
        local ipHourly = redis.call('GET', KEYS[4])
        local ipDestinationHourly = redis.call('GET', KEYS[5])
        local cooldownTtl = redis.call('TTL', KEYS[6])
        local destinationHourlyLimit = tonumber(ARGV[1])
        local destinationDailyLimit = tonumber(ARGV[2])
        local ipTenMinuteLimit = tonumber(ARGV[3])
        local ipHourlyLimit = tonumber(ARGV[4])
        local ipDestinationHourlyLimit = tonumber(ARGV[5])

        if cooldownTtl > 0 then
            return { 6, cooldownTtl }
        end

        if destinationHourly ~= false
           and tonumber(destinationHourly) >= destinationHourlyLimit then

            local ttl = redis.call('TTL', KEYS[1])
            if ttl < 0 then
                ttl = 0
            end

            return { 1, ttl }
        end

        if destinationDaily ~= false
           and tonumber(destinationDaily) >= destinationDailyLimit then

            local ttl = redis.call('TTL', KEYS[2])
            if ttl < 0 then
                ttl = 0
            end
            return { 2, ttl }
        end

        if ipTenMinute ~= false
           and tonumber(ipTenMinute) >= ipTenMinuteLimit then

            local ttl = redis.call('TTL', KEYS[3])
            if ttl < 0 then
                ttl = 0
            end
            return { 3, ttl }
        end

        if ipHourly ~= false
           and tonumber(ipHourly) >= ipHourlyLimit then

            local ttl = redis.call('TTL', KEYS[4])
            if ttl < 0 then
                ttl = 0
            end
            return { 4, ttl }
        end

        if ipDestinationHourly ~= false
           and tonumber(ipDestinationHourly) >= ipDestinationHourlyLimit then

            local ttl = redis.call('TTL', KEYS[5])
            if ttl < 0 then
                ttl = 0
            end
            return { 5, ttl }
        end

        local value

        value = redis.call('INCR', KEYS[1])
        if value == 1 then
            redis.call(
                'EXPIRE',
                KEYS[1],
                ARGV[6]
            )
        end

        value = redis.call('INCR', KEYS[2])
        if value == 1 then
            redis.call(
                'EXPIRE',
                KEYS[2],
                ARGV[7]
            )
        end

        value = redis.call('INCR', KEYS[3])
        if value == 1 then
            redis.call(
                'EXPIRE',
                KEYS[3],
                ARGV[8]
            )
        end

        value = redis.call('INCR', KEYS[4])
        if value == 1 then
            redis.call(
                'EXPIRE',
                KEYS[4],
                ARGV[9]
            )
        end

        value = redis.call('INCR', KEYS[5])
        if value == 1 then
            redis.call(
                'EXPIRE',
                KEYS[5],
                ARGV[10]
            )
        end

        redis.call(
            'SET',
            KEYS[6],
            '1',
            'EX',
            ARGV[11]
        )

        return { 0, 0 }
        """;

    private readonly IConnectionMultiplexer _redis;
    private readonly OtpRateLimitOptions _options;

    public RedisOtpRateLimiter(
        IConnectionMultiplexer redis,
        IOptions<OtpRateLimitOptions> options)
    {
        _redis = redis;
        _options = options.Value;
    }

    public async Task<OtpRateLimitDecision> CheckAndConsumeAsync(
        string destinationHash,
        string ipHash,
        string ipDestinationHash,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipDestinationHash);

        var db = _redis.GetDatabase();

        var keys = new RedisKey[]
        {
            BuildDestinationHourlyKey(destinationHash),
            BuildDestinationDailyKey(destinationHash),
            BuildIpTenMinuteKey(ipHash),
            BuildIpHourlyKey(ipHash),
            BuildIpDestinationHourlyKey(ipDestinationHash),
            BuildCooldownKey(destinationHash)
        };

        var result = await db.ScriptEvaluateAsync(
            RateLimitScript,
            keys,
            new RedisValue[]
            {
                _options.DestinationHourlyLimit,
                _options.DestinationDailyLimit,
                _options.IpTenMinuteLimit,
                _options.IpHourlyLimit,
                _options.IpDestinationHourlyLimit,

                (long)DestinationHourlyWindow.TotalSeconds,
                (long)DestinationDailyWindow.TotalSeconds,
                (long)IpTenMinuteWindow.TotalSeconds,
                (long)IpHourlyWindow.TotalSeconds,
                (long)IpDestinationHourlyWindow.TotalSeconds,

                (long)ResendCooldown.TotalSeconds
            });

        var values = (RedisResult[])result;

        var resultCode = (int)values[0];
        var retryAfter = Math.Max(0, (int)values[1]);

        var rateLimitResult = resultCode switch
        {
            0 => OtpRateLimitResult.Allowed,

            1 => OtpRateLimitResult
                .DestinationHourlyLimitExceeded,

            2 => OtpRateLimitResult
                .DestinationDailyLimitExceeded,

            3 => OtpRateLimitResult
                .IpTenMinuteLimitExceeded,

            4 => OtpRateLimitResult
                .IpHourlyLimitExceeded,

            5 => OtpRateLimitResult
                .IpDestinationHourlyLimitExceeded,

            6 => OtpRateLimitResult
                .ResendCooldownActive,

            _ => throw new InvalidOperationException(
                $"Unexpected OTP rate limit result: {resultCode}")
        };

        return new OtpRateLimitDecision(
            Allowed: rateLimitResult == OtpRateLimitResult.Allowed,
            Reason: rateLimitResult,
            RetryAfter: retryAfter);
    }

    private static RedisKey BuildDestinationHourlyKey(
        string destinationHash)
        => $"{KeyPrefix}destination:{destinationHash}:hour";

    private static RedisKey BuildDestinationDailyKey(
        string destinationHash)
        => $"{KeyPrefix}destination:{destinationHash}:day";

    private static RedisKey BuildIpTenMinuteKey(
        string ipHash)
        => $"{KeyPrefix}ip:{ipHash}:10min";

    private static RedisKey BuildIpHourlyKey(
        string ipHash)
        => $"{KeyPrefix}ip:{ipHash}:hour";

    private static RedisKey BuildIpDestinationHourlyKey(
        string ipDestinationHash)
        => $"{KeyPrefix}ip-destination:{ipDestinationHash}:hour";

    private static RedisKey BuildCooldownKey(
        string destinationHash)
        => $"{KeyPrefix}cooldown:{destinationHash}";
}