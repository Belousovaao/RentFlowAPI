using System;

namespace RentFlow.Domain.Assets;

public enum AssetStatus
{
    Available = 1,
    Reserved = 2,
    InRent = 3,
    Service = 4,
    Disabled = 5
}
