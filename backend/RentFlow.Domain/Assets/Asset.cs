using System;
using System.Diagnostics;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Locations;

namespace RentFlow.Domain.Assets;

public class Asset
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string BrandName { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string FullDescription { get; private set; } = string.Empty;

    public AssetType Type { get; private set; }
    public AssetCategory Category { get; private set; }

    public int Year { get; private set; }
    public FuelType FuelType { get; private set; }
    public TransmissionType Transmission { get; private set; }
    public DriveType DriveType { get; private set; }
    public int Doors { get; private set; } 
    public int Seats { get; private set; }
    public string Engine { get; private set; }
    public int Horsepower { get; private set; }
    public string Acceleration { get; private set; }
    public int TopSpeed { get; private set; }
    public string Color { get; private set; }
    public List<string> Features { get; private set; } = new();


    public decimal DailyPrice { get; private set; }
    public decimal? Deposit { get; private set; }

    public Guid LocationId { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public AssetStatus Status { get; set; }

    private readonly List<AssetPhoto> _photos = new();
    public IReadOnlyCollection<AssetPhoto> Photos => _photos;
    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings;

    private Asset() {}

    public Asset(
        string code,
        string brandName,
        string model,
        string shortDescription,
        string fullDescription,
        AssetType type,
        AssetCategory category,
        int year,
        FuelType fuelType,
        TransmissionType transmission,
        DriveType driveType,
        int seats,
        int doors,
        string engine,
        int horsepower,
        string acceleration,
        int topSpeed,
        string color,
        List<string> features,
        decimal dailyPrice,
        decimal? deposit,
        Guid locationId,
        double? latitude,
        double? longitude
    )
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.");

        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("BrandName is required.");

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.");
        
        if (dailyPrice <= 0)
            throw new ArgumentException("DailyPrice cant't be 0 or less.");

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Invalid year.");

        if (seats <= 0)
            throw new ArgumentException("Seats must be greater than 0.");

        if (doors <= 0)
            throw new ArgumentException("Doors must be greater than 0.");

        if (horsepower <= 0)
            throw new ArgumentException("Horsepower must be greater than 0.");

        if (topSpeed <= 0)
            throw new ArgumentException("Top speed must be greater than 0.");

        Console.WriteLine($"Asset constructor - BEFORE: Code = {code}");
            
        Id = Guid.NewGuid();
        Code = code;
        Console.WriteLine($"Asset constructor - AFTER: Code = {Code}");
        BrandName = brandName;
        Model = model;
        ShortDescription = shortDescription;
        FullDescription = fullDescription;
        Type = type;
        Category = category;
        Year = year;
        FuelType = fuelType;
        Transmission = transmission;
        DriveType = driveType;
        Seats = seats;
        Doors = doors;
        Engine = engine;
        Horsepower = horsepower;
        Acceleration = acceleration;
        TopSpeed = topSpeed;
        Color = color;
        Features = features;
        DailyPrice = dailyPrice;
        Deposit = deposit;
        LocationId = locationId; 
        Status = AssetStatus.Available;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Asset Create(
        string code,
        string brandName,
        string model,
        string shortDescription,
        string fullDescription,
        AssetType type,
        AssetCategory category,
        int year,
        FuelType fuelType,
        TransmissionType transmission,
        DriveType driveType,
        int seats,
        int doors,
        string engine,
        int horsepower,
        string acceleration,
        int topSpeed,
        string color,
        List<string> features,
        decimal dailyPrice,
        decimal? deposit,
        Guid locationId,
        double? latitude,
        double? longitude
    )
    {
        return new Asset(code, brandName, model, shortDescription,
            fullDescription, type, category, year, fuelType, transmission, driveType, seats, doors,
            engine, horsepower, acceleration, topSpeed, color, features, dailyPrice, deposit,
            locationId, latitude, longitude);
    }

    public void Update(
        string brandName,
        string model,
        string shortDescription,
        string fullDescription,
        AssetCategory category,
        int year,
        FuelType fuelType,
        TransmissionType transmission,
        DriveType driveType,
        int seats,
        int doors,
        string engine,
        int horsepower,
        string acceleration,
        int topSpeed,
        string color,
        List<string> features,
        decimal dailyPrice,
        decimal? deposit,
        AssetStatus status,
        double? latitude,
        double? longitude)
    {
        BrandName = brandName;
        Model = model;
        ShortDescription = shortDescription;
        FullDescription = fullDescription;
        Category = category;
        Year = year;

        FuelType = fuelType;
        Transmission = transmission;
        DriveType = driveType;

        Seats = seats;
        Doors = doors;

        Engine = engine;
        Horsepower = horsepower;
        Acceleration = acceleration;
        TopSpeed = topSpeed;
        Color = color;
        Features = features;

        DailyPrice = dailyPrice;
        Deposit = deposit;

        Status = status;

        Latitude = latitude;
        Longitude = longitude;
    }
    public void AddPhoto(string url, PhotoType type, int order = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo url required");
        
        _photos.Add(new AssetPhoto(Id, url, type, order));
    }

    public IReadOnlyCollection<AssetPhoto> GetExteriorPhotos() 
        => _photos.Where(p => p.Type == PhotoType.Exterior).OrderBy(p => p.Order).ToList();
    
    public IReadOnlyCollection<AssetPhoto> GetInteriorPhotos() 
        => _photos.Where(p => p.Type == PhotoType.Interior).OrderBy(p => p.Order).ToList();

    public void UpdateStatus(AssetStatus newStatus)
    {
        Status = newStatus;
    }

    public void UpdateLocation(Guid locationId, double? latitude, double? longitude)
    {
        LocationId = locationId;
        Latitude = latitude;
        Longitude = longitude;
    }
}
