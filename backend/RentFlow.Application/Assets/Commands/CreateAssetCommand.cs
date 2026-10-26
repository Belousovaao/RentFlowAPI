using MediatR;
using RentFlow.Domain.Assets;
using DriveType = RentFlow.Domain.Assets.DriveType;

namespace RentFlow.Application.Assets.Commands;
public class CreateAssetCommand : IRequest<Asset>
{
    public string BrandName { get; set; }
    public string Model { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public AssetType Type { get; set; }
    public AssetCategory Category { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    public DriveType DriveType { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
    public string Engine { get; set; }
    public int Horsepower { get; set; }
    public string Acceleration { get; set; }
    public int TopSpeed { get; set; }
    public string Color { get; set; }
    public List<string> Features { get; set; }
    public decimal? Deposit { get; set; }
    public string Status { get; set; }
    public Guid? LocationId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}