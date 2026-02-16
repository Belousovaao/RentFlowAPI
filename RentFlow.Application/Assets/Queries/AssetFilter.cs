using System;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Queries;

public class AssetFilter
{
    public AssetType? Type { get; set; }
    public string? Search { get; set; }      // code + name
    public AssetCategory? Category { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public Guid? LocationId { get; set; }
    public AssetStatus? Status { get; set; }
}
