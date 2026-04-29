namespace RentFlow.Application.Assets.Queries;
public class AdminAssetFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Brand { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? ShowDisabled { get; set; }
}