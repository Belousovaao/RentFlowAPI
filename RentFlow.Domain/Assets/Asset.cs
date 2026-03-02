using System;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Locations;

namespace RentFlow.Domain.Assets;

public class Asset
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string BrandName { get; private set; }
    public string Model { get; private set; }
    public string ShortDescription { get; private set; }
    public string FullDescription { get; private set; }

    public AssetType Type { get; private set; }
    public AssetCategory Category { get; private set; }

    public decimal DailyPrice { get; private set; }
    public decimal? Deposit { get; private set; }

    public Guid LocationId { get; private set; }
    public bool CanDeliver { get; private set; }
    public decimal? DeliveryPrice { get; private set; }

    public AssetStatus Status { get; private set; }

    private readonly List<AssetPhoto> _photos = new();
    public IReadOnlyCollection<AssetPhoto> Photos => _photos;

    private Asset() {}

    public Asset(
        string code,
        string brandName,
        string model,
        string shortDescription,
        string fullDescription,
        AssetType type,
        AssetCategory category,
        decimal dailyPrice,
        decimal? deposit,
        Guid locationId,
        bool canDeliver,
        decimal? deliveryPrice
    )
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.");

        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("BrandName is required.");

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.");
        
        if (dailyPrice <= 0)
            throw new ArgumentException("DailyPrice cant't be 0 or less.");

        if (canDeliver && deliveryPrice is null)
            throw new ArgumentException("DeliveryPrice required id delivery is able");

        Code = code;
        BrandName = brandName;
        Model = model;
        ShortDescription = shortDescription;
        FullDescription = fullDescription;
        Type = type;
        Category = category;
        DailyPrice = dailyPrice;
        Deposit = deposit;
        LocationId = locationId; 
        CanDeliver = canDeliver;
        DeliveryPrice = deliveryPrice;
        Status = AssetStatus.Available;
    }

    public static Asset Create(
        string code,
        string brandName,
        string model,
        string shortDescription,
        string fullDescription,
        AssetType type,
        AssetCategory category,
        decimal dailyPrice,
        decimal? deposit,
        Guid locationId,
        bool canDeliver,
        decimal? deliveryPrice
    )
    {
        return new Asset(code, brandName, model, shortDescription,
            fullDescription, type, category, dailyPrice, deposit,
            locationId, canDeliver, deliveryPrice);
    }

        public void AddPhoto(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo url required");

        _photos.Add(new AssetPhoto(Id, url));
    }
}
