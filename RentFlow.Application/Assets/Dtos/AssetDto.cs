using System;

namespace RentFlow.Application.Assets.Dtos;

public sealed record AssetDto(
    Guid Id,
    string Code,
    string BrandName,
    string Model,
    decimal DailyPrice
);
