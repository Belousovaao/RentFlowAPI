using System;
using System.Text.Json.Serialization;

namespace RentFlow.Domain.Assets;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetStatus
{
    Available = 1,
    Reserved = 2,
    InRent = 3,
    Service = 4,
    Disabled = 5
}
