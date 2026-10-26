using RentFlow.Application.Assets.Dtos;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Mappers;

public static class AssetMapper
{
    public static AssetDto ToDto(Asset asset)
    {
        return new AssetDto(
            Id: asset.Id,
            Code: asset.Code,
            BrandName: asset.BrandName,
            Model: asset.Model,
            Year: asset.Year,
            ShortDescription: asset.ShortDescription,
            FullDescription: asset.FullDescription,
            Type: asset.Type,
            Category: asset.Category,
            FuelType: asset.FuelType,
            Transmission: asset.Transmission,
            DriveType: asset.DriveType,
            Seats: asset.Seats,
            Doors: asset.Doors,
            Engine: asset.Engine,
            Horsepower: asset.Horsepower,
            Acceleration: asset.Acceleration,
            TopSpeed: asset.TopSpeed,
            Color: asset.Color,
            Features: asset.Features,
            DailyPrice: asset.DailyPrice,
            Deposit: asset.Deposit,
            Status: asset.Status,
            LocationId: asset.LocationId,

            // Пока Location не загружается
            LocationName: string.Empty,
            Latitude: asset.Latitude,
            Longitude: asset.Longitude,
            DistanceFromUser: null,

            Photos: asset.Photos
                .OrderBy(x => x.Order)
                .Select(ToPhotoDto)
                .ToList(),

            ExteriorPhotos: asset.GetExteriorPhotos()
                .Select(ToPhotoDto)
                .ToList(),

            InteriorPhotos: asset.GetInteriorPhotos()
                .Select(ToPhotoDto)
                .ToList()
        );
    }

    private static PhotoDto ToPhotoDto(AssetPhoto photo)
    {
        return new PhotoDto(
            photo.Id,
            photo.Url,
            photo.Type.ToString(),
            photo.Order
        );
    }
}