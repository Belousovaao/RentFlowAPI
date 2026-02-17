using System;
using RentFlow.Domain.Assets;

namespace RentFlow.Domain.Bookings;

public class BookingAssetSnapshot
{
    public string Name { get; set; }
    public AssetType Type { get; set; }
    public AssetCategory Category { get; set; }
    public decimal DailyPrice { get; set; }
    public decimal Deposit { get; set; }
    public bool CanDeliver { get; set; }
    public decimal? DeliveryPrice { get; set; }
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
