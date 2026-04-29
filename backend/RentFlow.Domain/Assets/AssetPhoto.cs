using System;

namespace RentFlow.Domain.Assets;

public class AssetPhoto
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public string Url { get; private set; }
    public PhotoType Type { get; private set; } // Exterior или Interior
    public int Order { get; private set; }

    private AssetPhoto() {}

    public AssetPhoto(Guid assetId, string url, PhotoType type, int order = 0)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        Url = url;
        Type = type;
        Order = order;
    }

    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
    }
}


public enum PhotoType
{
    Exterior = 1,
    Interior = 2
}