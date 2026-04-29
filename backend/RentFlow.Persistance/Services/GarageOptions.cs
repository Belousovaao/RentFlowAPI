namespace RentFlow.Persistance.Services;

public sealed class GarageOptions
{
    public const string SectionName = "Garage";

    public string Endpoint { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicBaseUrl { get; set; } = string.Empty;
}
