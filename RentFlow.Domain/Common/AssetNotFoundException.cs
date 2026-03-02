using System;

namespace RentFlow.Domain.Common;

public sealed class AssetNotFoundException : DomainException
{
    public AssetNotFoundException() : base("Asset not found.")
    {
    }
}
