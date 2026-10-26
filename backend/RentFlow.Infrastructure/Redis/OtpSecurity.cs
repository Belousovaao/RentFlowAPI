using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RentFlow.Application.Interfaces;

namespace RentFlow.Infrastructure.Redis;

public sealed class OtpSecurity : IOtpSecurity
{
    private const string CodeContext = "otp-code:";
    private const string DestinationContext = "otp-destination:";
    private const string IpContext = "otp-ip:";
    private const string IpDestinationContext = "otp-ip-destination:";

    private readonly byte[] _hmacKey;

    public OtpSecurity(IOptions<OtpSecurityOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var key = options.Value.HmacKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("OtpSecurity:HmacKey is not configured.");
        }
        _hmacKey = ConvertKey(key);
    }

    public string GenerateCode()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return number.ToString("D6");
    }

    public string HashCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return ComputeHmac(CodeContext, code);
    }

    public string HashDestination(string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        return ComputeHmac(DestinationContext, destination);
    }

    public string HashIp(string ipAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        return ComputeHmac(IpContext, ipAddress);
    }

    public string HashIpDestination(string ipAddress, string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var value = $"{ipAddress}\n{destination}";
        return ComputeHmac(IpDestinationContext, value);
    }

    private string ComputeHmac(string context, string value)
    {
        var payload = Encoding.UTF8.GetBytes(context + value);
        var hash = HMACSHA256.HashData(_hmacKey, payload);
        return Convert.ToHexString(hash);
    }

    private static byte[] ConvertKey(string value)
    {
        try
        {
            var key = Convert.FromBase64String(value);
            if (key.Length < 32)
            {
                throw new InvalidOperationException("OtpSecurity:HmacKey must contain at least 32 bytes.");
            }
            return key;
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("OtpSecurity:HmacKey must be a valid Base64 string.");
        }
    }
}