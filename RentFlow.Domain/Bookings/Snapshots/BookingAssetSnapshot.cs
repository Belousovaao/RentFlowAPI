using System;
using RentFlow.Domain.Assets;

namespace RentFlow.Domain.Bookings;

public class BookingAssetSnapshot
{
    public string Name { get; }
    public AssetType Type { get; }
    public AssetCategory Category { get; }
    public decimal DailyPrice { get; }
    public decimal Deposit { get; }
    public bool CanDeliver { get; }
    public decimal? DeliveryPrice { get; }
    private BookingAssetSnapshot() {}
    public BookingAssetSnapshot(string name, AssetType type, 
        AssetCategory category, decimal dailePrice, decimal deposit, 
        bool canDeliver, decimal? deliveryPrice)
    {
        Name = name;
        Type = type;
        Category = category;
        DailyPrice = dailePrice;
        Deposit = deposit;
        CanDeliver = canDeliver;
        DeliveryPrice = deliveryPrice;
    }
}
