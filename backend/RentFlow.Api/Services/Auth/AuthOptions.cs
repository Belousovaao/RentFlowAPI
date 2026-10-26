namespace RentFlow.Api.Services.Auth;

public class AuthOptions
{
    public const string SectionName = "Auth";
    public string AdminEmail { get; set; } = "admin@rentflow.local";
    public string AdminPhone { get; set; } = "+79000000000";
    public string TestUserEmail { get; set; } = "user@rentflow.local";
    public string TestUserPhone { get; set; } = "+79001112233";
    public int CodeLength { get; set; } = 6;
    public int CodeExpiryMinutes { get; set; } = 10;
    /// <summary>In Development, verification codes are returned in API responses.</summary>
    public bool ExposeCodesInResponse { get; set; } = true;
}
