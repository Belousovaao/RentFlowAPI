using System.Security.Claims;

namespace RentFlow.Api.Services.Auth;

public static class ClsimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (value is null)
            throw new UnauthorizedAccessException();

        return Guid.Parse(value);
    }

    public static Guid GetCustomerId(this ClaimsPrincipal user)
    {

        var value = user.FindFirst("customerId")?.Value;

        if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var customerId))
        {
            throw new UnauthorizedAccessException("CustomerId claim is missing or invalid.");
        }

        return customerId;
    }
}