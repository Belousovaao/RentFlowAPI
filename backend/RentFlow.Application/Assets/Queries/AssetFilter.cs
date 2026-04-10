using System;
using RentFlow.Domain.Assets;
using DriveType = RentFlow.Domain.Assets.DriveType;

namespace RentFlow.Application.Assets.Queries;

public class AssetFilter
{
    public AssetType? Type { get; set; }
    public string? Search { get; set; }      // code + name
    public AssetCategory? Category { get; set; }
     public string? Brand { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public FuelType? FuelType { get; set; }
    public TransmissionType? Transmission { get; set; }
    public DriveType? DriveType { get; set; }
    public int? MinSeats { get; set; }
    public int? MaxSeats { get; set; }
    public int? MinDoors { get; set; }
    public int? MaxDoors { get; set; }
    public int? MinHorsepower { get; set; }
    public int? MaxHorsepower { get; set; }
    public Guid? LocationId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AssetStatus? Status { get; set; }
     public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
    public string? SortBy { get; set; }
}
