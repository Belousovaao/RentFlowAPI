using System;
using RentFlow.Domain.Assets;

namespace RentFlow.Domain.Bookings;

public class BookingAssetSnapshot
{
    public string Name { get; private set; }
    public AssetType Type { get; private set; }
    public AssetCategory Category { get; private set; }
    public decimal DailyPrice { get; private set; }
    public decimal? Deposit { get; private set; }
    public bool CanDeliver { get; private set; }
    public decimal? DeliveryPrice { get; private set; }
    private BookingAssetSnapshot() {}
    public BookingAssetSnapshot(string brandName, string model, AssetType type, 
        AssetCategory category, decimal dailePrice, decimal? deposit, 
        bool canDeliver, decimal? deliveryPrice)
    {
        Name = brandName + " " + model;
        Type = type;
        Category = category;
        DailyPrice = dailePrice;
        Deposit = deposit;
        CanDeliver = canDeliver;
        DeliveryPrice = deliveryPrice;
    }
}
