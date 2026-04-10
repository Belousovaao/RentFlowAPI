using RentFlow.Domain.Assets;
using DriveType = RentFlow.Domain.Assets.DriveType;

namespace RentFlow.Application.Assets.Dtos;

public sealed record AssetDto(
    Guid Id,
    string Code,
    string BrandName,
    string Model,
    int Year,
    string ShortDescription,
    string FullDescription,
    AssetType Type,
    AssetCategory Category,
    FuelType FuelType,
    TransmissionType Transmission,
    DriveType DriveType,
    int Seats,
    int Doors,
    string Engine,
    int Horsepower,
    string Acceleration,
    int TopSpeed,
    string Color,
    IReadOnlyList<string> Features,
    decimal DailyPrice,
    decimal? Deposit,
    AssetStatus Status,
    Guid LocationId,
    string LocationName,
    double? Latitude,
    double? Longitude,
    double? DistanceFromUser,
    IReadOnlyList<string> Photos,
    bool IsAvailable
);

public sealed record AssetSpecificationsDto(
    string Engine,
    int Horsepower,
    string Acceleration,
    int TopSpeed,
    string Color,
    IReadOnlyList<string> Features
);