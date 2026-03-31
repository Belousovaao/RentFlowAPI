using System;

namespace RentFlow.Domain.Assets;

public class AssetPhoto
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public string Url { get; private set; }

    private AssetPhoto() {}

    internal AssetPhoto(Guid assetId, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo url required");
        
        Id = Guid.NewGuid();
        AssetId = assetId;
        Url = url;
    }
}
