namespace RentFlow.Application.Assets.Dtos;
/// <summary>
/// Краткая информация об авто для отображения в бронировании
/// </summary>
/// <param name="Id"></param>
/// <param name="BrandName"></param>
/// <param name="Model"></param>
/// <param name="Year"></param>
/// <param name="CoverPhotoUrl"></param>
public sealed record AssetShortDto(
    Guid Id,
    string BrandName,
    string Model,
    int Year,
    string? CoverPhotoUrl
);