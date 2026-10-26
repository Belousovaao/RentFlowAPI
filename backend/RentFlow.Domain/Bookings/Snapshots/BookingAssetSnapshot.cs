using System;
using RentFlow.Domain.Assets;

namespace RentFlow.Domain.Bookings.Snapshots;

public class BookingAssetSnapshot
{
    public string BrandName { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public int Year { get; private set; }
    public AssetType Type { get; private set; }
    public AssetCategory Category { get; private set; }
    private BookingAssetSnapshot() {}
    public BookingAssetSnapshot(string brandName, string model, int year, AssetType type, 
        AssetCategory category)
    {
        BrandName = brandName;
        Model = model;
        Year = year;
        Type = type;
        Category = category;
    }
}
