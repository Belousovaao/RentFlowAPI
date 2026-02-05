using System;

namespace RentFlow.Domain.Assets;

public class AssetPhoto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string Url { get; set; }

    public Asset Asset { get; set; }
}
