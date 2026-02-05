using System;
using RentFlow.Domain.Locations;

namespace RentFlow.Domain.Assets;

public class Asset
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string FullDescription { get; private set; }

    public AssetType Type { get; private set; }
    public string Category { get; private set; }

    public decimal DailyPrice { get; private set; }
    public decimal Deposit { get; private set; }

    public Guid LocationId { get; private set; }
     public Location Location { get; set; } 
    public bool CanDeliver { get; private set; }
    public decimal? DeliveryPrice { get; private set; }

    public AssetStatus Status { get; private set; }

    private readonly List<AssetPhoto> _photos = new();
    public IReadOnlyCollection<AssetPhoto> Photos => _photos;
}
