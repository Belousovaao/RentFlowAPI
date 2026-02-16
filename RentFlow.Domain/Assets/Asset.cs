using System;
using RentFlow.Domain.Locations;

namespace RentFlow.Domain.Assets;

public class Asset
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }

    public AssetType Type { get; set; }
    public AssetCategory Category { get; set; }

    public decimal DailyPrice { get; set; }
    public decimal Deposit { get; set; }

    public Guid LocationId { get; set; }
     public Location Location { get; set; } 
    public bool CanDeliver { get; set; }
    public decimal? DeliveryPrice { get; set; }

    public AssetStatus Status { get; set; }

    private readonly List<AssetPhoto> _photos = new();
    public IReadOnlyCollection<AssetPhoto> Photos => _photos;
}
