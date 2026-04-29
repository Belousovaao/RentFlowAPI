using MediatR;
using RentFlow.Domain.Assets;
using DriveType = RentFlow.Domain.Assets.DriveType;

namespace RentFlow.Application.Assets.Commands;
public sealed record CreateAssetCommand(
    string BrandName,
    string Model,
    int Year,
    decimal DailyPrice,
    FuelType FuelType,
    TransmissionType Transmission,
    DriveType DriveType,
    int Seats,
    int Horsepower,
    AssetStatus Status
) : IRequest<Asset>;