using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RentFlow.Api.Services.Auth;

public interface IVerificationCodeService
{
    string GenerateAndStore(string channel, string destination);
    bool Validate(string channel, string destination, string code, bool consume = true);
}

public class VerificationCodeService : IVerificationCodeService
{
    private readonly IMemoryCache _cache;
    private readonly AuthOptions _options;
    private readonly ILogger<VerificationCodeService> _logger;

    public VerificationCodeService(
        IMemoryCache cache,
        IOptions<AuthOptions> options,
        ILogger<VerificationCodeService> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public string GenerateAndStore(string channel, string destination)
    {
        var code = GenerateCode();
        var key = CacheKey(channel, destination);
        _cache.Set(key, code, TimeSpan.FromMinutes(_options.CodeExpiryMinutes));
        _logger.LogInformation("Verification code for {Channel} {Destination}: {Code}", channel, destination, code);
        return code;
    }

    public bool Validate(string channel, string destination, string code, bool consume = true)
    {
        var key = CacheKey(channel, destination);
        if (!_cache.TryGetValue(key, out string? stored) || stored != code.Trim())
            return false;

        if (consume)
            _cache.Remove(key);

        return true;
    }

    private string GenerateCode()
    {
        var max = (int)Math.Pow(10, _options.CodeLength);
        var value = Random.Shared.Next(0, max);
        return value.ToString().PadLeft(_options.CodeLength, '0');
    }

    private static string CacheKey(string channel, string destination) =>
        $"auth-code:{channel}:{destination.ToLowerInvariant()}";
}
