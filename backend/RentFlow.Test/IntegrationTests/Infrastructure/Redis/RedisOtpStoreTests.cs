using FluentAssertions;
using RentFlow.Application.Auth.Otp;
using RentFlow.Infrastructure.Redis;
using StackExchange.Redis;

namespace RentFlow.Test.IntegrationTests.Infrastructure.Redis;

public sealed class RedisOtpStoreTests
    : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _fixture;

    public RedisOtpStoreTests(RedisContainerFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<(RedisOtpStore Store, IConnectionMultiplexer Redis)>
        CreateStoreAsync()
    {
        var redis =
            await ConnectionMultiplexer.ConnectAsync(
                _fixture.ConnectionString);

        return (
            new RedisOtpStore(redis),
            redis);
    }

    private static OtpChallenge CreateChallenge(
        Guid? challengeId = null,
        string? destinationHash = null,
        string codeHash = "code-hash")
    {
        return new OtpChallenge
        {
            ChallengeId = challengeId ?? Guid.NewGuid(),
            Channel = "email",
            DestinationHash = destinationHash ?? $"destination-{Guid.NewGuid():N}",
            CodeHash = codeHash,
            Attempts = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    [Fact]
    public async Task VerifyAsync_WithCorrectCode_ReturnsSuccess()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var challenge = CreateChallenge();

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                challenge.CodeHash,
                maxAttempts: 5);

            result.Should().Be(
                OtpVerificationResult.Success);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WithUnknownChallenge_ReturnsNotFound()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var result = await store.VerifyAsync(
                Guid.NewGuid(),
                "unknown-destination",
                "some-code-hash",
                maxAttempts: 5);

            result.Should().Be(
                OtpVerificationResult.NotFound);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WithInvalidCode_IncrementsAttempts()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var challenge = CreateChallenge(
                codeHash: "correct-code-hash");

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                "wrong-code-hash",
                maxAttempts: 5);

            result.Should().Be(
                OtpVerificationResult.InvalidCode);

            var secondResult = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                "wrong-code-hash",
                maxAttempts: 5);

            secondResult.Should().Be(
                OtpVerificationResult.InvalidCode);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_OnFifthInvalidAttempt_ReturnsTooManyAttempts()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var challenge = CreateChallenge(
                codeHash: "correct-code-hash");

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            for (var attempt = 1; attempt <= 4; attempt++)
            {
                var result = await store.VerifyAsync(
                    challenge.ChallengeId,
                    challenge.DestinationHash,
                    "wrong-code-hash",
                    maxAttempts: 5);

                result.Should().Be(
                    OtpVerificationResult.InvalidCode);
            }

            var fifthResult = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                "wrong-code-hash",
                maxAttempts: 5);

            fifthResult.Should().Be(
                OtpVerificationResult.TooManyAttempts);

            var afterLockoutResult = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                "correct-code-hash",
                maxAttempts: 5);

            afterLockoutResult.Should().Be(
                OtpVerificationResult.NotFound);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task CreateAsync_WhenNewChallengeIsCreated_InvalidatesPreviousChallenge()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            const string destinationHash = "same-destination";

            var firstChallenge = CreateChallenge(
                destinationHash: destinationHash,
                codeHash: "first-code");

            var secondChallenge = CreateChallenge(
                destinationHash: destinationHash,
                codeHash: "second-code");

            await store.CreateAsync(
                firstChallenge,
                TimeSpan.FromMinutes(5));

            await store.CreateAsync(
                secondChallenge,
                TimeSpan.FromMinutes(5));

            var firstResult = await store.VerifyAsync(
                firstChallenge.ChallengeId,
                destinationHash,
                "first-code",
                maxAttempts: 5);

            firstResult.Should().Be(
                OtpVerificationResult.NotFound);

            var secondResult = await store.VerifyAsync(
                secondChallenge.ChallengeId,
                destinationHash,
                "second-code",
                maxAttempts: 5);

            secondResult.Should().Be(
                OtpVerificationResult.Success);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WhenChallengeExpires_ReturnsNotFound()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var challenge = CreateChallenge();

            await store.CreateAsync(
                challenge,
                TimeSpan.FromSeconds(1));

            await Task.Delay(
                TimeSpan.FromSeconds(2));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                challenge.DestinationHash,
                challenge.CodeHash,
                maxAttempts: 5);

            result.Should().Be(
                OtpVerificationResult.NotFound);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WhenCalledConcurrentlyWithCorrectCode_AllowsOnlyOneSuccess()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var challenge = CreateChallenge(
                codeHash: "correct-code");

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            const int concurrentAttempts = 10;

            var tasks = Enumerable
                .Range(0, concurrentAttempts)
                .Select(_ =>
                    store.VerifyAsync(
                        challenge.ChallengeId,
                        challenge.DestinationHash,
                        "correct-code",
                        maxAttempts: 5))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            results.Count(
                    result =>
                        result == OtpVerificationResult.Success)
                .Should()
                .Be(1);

            results.Count(
                    result =>
                        result == OtpVerificationResult.NotFound)
                .Should()
                .Be(concurrentAttempts - 1);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WithWrongDestination_ReturnsNotFound()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var destinationHash =
                $"destination-{Guid.NewGuid():N}";

            var wrongDestinationHash =
                $"destination-{Guid.NewGuid():N}";

            var challenge = CreateChallenge(
                destinationHash: destinationHash);

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                wrongDestinationHash,
                challenge.CodeHash,
                maxAttempts: 5);

            result.Should()
                .Be(OtpVerificationResult.NotFound);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WithWrongDestination_DoesNotConsumeAttempt()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var destinationHash =
                $"destination-{Guid.NewGuid():N}";

            var wrongDestinationHash =
                $"destination-{Guid.NewGuid():N}";

            var challenge = CreateChallenge(
                destinationHash: destinationHash);

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                wrongDestinationHash,
                "wrong-code",
                maxAttempts: 5);

            result.Should()
                .Be(OtpVerificationResult.NotFound);

            var success = await store.VerifyAsync(
                challenge.ChallengeId,
                destinationHash,
                challenge.CodeHash,
                maxAttempts: 5);

            success.Should()
                .Be(OtpVerificationResult.Success);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }

    [Fact]
    public async Task VerifyAsync_WithCorrectDestinationAndWrongCode_ReturnsInvalidCode()
    {
        var (store, redis) = await CreateStoreAsync();

        try
        {
            var destinationHash =
                $"destination-{Guid.NewGuid():N}";

            var challenge = CreateChallenge(
                destinationHash: destinationHash,
                codeHash: "correct-code");

            await store.CreateAsync(
                challenge,
                TimeSpan.FromMinutes(5));

            var result = await store.VerifyAsync(
                challenge.ChallengeId,
                destinationHash,
                "wrong-code",
                maxAttempts: 5);

            result.Should()
                .Be(OtpVerificationResult.InvalidCode);
        }
        finally
        {
            await redis.DisposeAsync();
        }
    }
}