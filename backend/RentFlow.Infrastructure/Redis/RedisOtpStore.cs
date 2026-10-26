using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using StackExchange.Redis;

namespace RentFlow.Infrastructure.Redis;

public sealed class RedisOtpStore : IOtpStore
{
    private const string KeyPrefix = "otp:challenge:";

    private const string VerifyScript = """
        local challengeKey = KEYS[1]

        local storedCodeHash = redis.call('HGET', challengeKey, 'codeHash')

        if not storedCodeHash then
            return 1
        end

        local destinationHash = redis.call('HGET', challengeKey, 'destinationHash')

        if not destinationHash then
            return 1
        end

        local providedDestinationHash = ARGV[3]

        if destinationHash ~= providedDestinationHash then
            return 1
        end

        local currentKey = 'otp:challenge:current:' .. destinationHash

        local currentChallengeId = redis.call('GET', currentKey)

        local challengeId = string.sub(
            challengeKey,
            string.len('otp:challenge:') + 1
        )

        if currentChallengeId ~= challengeId then
            redis.call('DEL', challengeKey)
            return 1
        end

        local attempts = tonumber(
            redis.call('HGET', challengeKey, 'attempts') or '0'
        )

        local maxAttempts = tonumber(ARGV[2])

        if attempts >= maxAttempts then
            redis.call('DEL', challengeKey)
            redis.call('DEL', currentKey)

            return 3
        end

        local providedCodeHash = ARGV[1]

        if storedCodeHash ~= providedCodeHash then
            local updatedAttempts = redis.call(
                'HINCRBY',
                challengeKey,
                'attempts',
                1
            )

            if tonumber(updatedAttempts) >= maxAttempts then
                redis.call('DEL', challengeKey)
                redis.call('DEL', currentKey)

                return 3
            end

            return 2
        end

        redis.call('DEL', challengeKey)
        redis.call('DEL', currentKey)

        return 0
        """;

    private const string CreateScript = """
        local currentKey = KEYS[1]
        local challengeKey = KEYS[2]

        local previousChallengeId =
            redis.call('GET', currentKey)

        if previousChallengeId then
            redis.call(
                'DEL',
                'otp:challenge:' .. previousChallengeId
            )
        end

        redis.call(
            'HSET',
            challengeKey,
            'channel',
            ARGV[1],
            'destinationHash',
            ARGV[2],
            'codeHash',
            ARGV[3],
            'attempts',
            ARGV[4],
            'createdAt',
            ARGV[5]
        )

        redis.call(
            'EXPIRE',
            challengeKey,
            ARGV[6]
        )

        redis.call(
            'SET',
            currentKey,
            ARGV[7],
            'EX',
            ARGV[6]
        )

        return 1
        """;

    private readonly IConnectionMultiplexer _redis;

    public RedisOtpStore(IConnectionMultiplexer redis)
    {
        ArgumentNullException.ThrowIfNull(redis);

        _redis = redis;
    }

    public async Task CreateAsync(
        OtpChallenge challenge,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(challenge);

        if (ttl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ttl),
                "OTP challenge TTL must be greater than zero.");
        }

        var db = _redis.GetDatabase();

        var challengeKey =
            BuildChallengeKey(challenge.ChallengeId);

        var currentKey =
            BuildCurrentKey(challenge.DestinationHash);

        await db.ScriptEvaluateAsync(
            CreateScript,
            new RedisKey[]
            {
                currentKey,
                challengeKey
            },
            new RedisValue[]
            {
                challenge.Channel,
                challenge.DestinationHash,
                challenge.CodeHash,
                challenge.Attempts,
                challenge.CreatedAt.ToUnixTimeSeconds(),
                (long)ttl.TotalSeconds,
                challenge.ChallengeId.ToString("N")
            });
    }

    public async Task<OtpVerificationResult> VerifyAsync(
        Guid challengeId,
        string destinationHash,
        string codeHash,
        int maxAttempts,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codeHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationHash);

        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts),
                "Maximum OTP attempts must be greater than zero.");
        }

        var db = _redis.GetDatabase();
        var challengeKey = BuildChallengeKey(challengeId);
        var result = await db.ScriptEvaluateAsync(
            VerifyScript,
            new RedisKey[]
            {
                challengeKey
            },
            new RedisValue[]
            {
                codeHash,
                maxAttempts,
                destinationHash
            });

        return (int)result switch
        {
            0 => OtpVerificationResult.Success,

            1 => OtpVerificationResult.NotFound,

            2 => OtpVerificationResult.InvalidCode,

            3 => OtpVerificationResult.TooManyAttempts,

            _ => throw new InvalidOperationException($"Unexpected OTP verification result: {result}")
        };
    }

    public async Task DeleteAsync(
        Guid challengeId,
        CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        await db.KeyDeleteAsync(
            BuildChallengeKey(challengeId));
    }

    private static RedisKey BuildChallengeKey(
        Guid challengeId)
        => $"{KeyPrefix}{challengeId:N}";

    private static RedisKey BuildCurrentKey(
        string destinationHash)
        => $"{KeyPrefix}current:{destinationHash}";
}