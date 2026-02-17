using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Locations;
using RentFlow.Domain.Bookings;

namespace RentFlow.Persistance;

public class RentFlowDbContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<AssetPhoto> AssetPhotos => Set<AssetPhoto>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<RentalPeriod> RentalPeriods => Set<RentalPeriod>();
    public DbSet<PricingRule> PricingRules => Set<PricingRule>();

    public RentFlowDbContext(DbContextOptions<RentFlowDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assetId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var customerId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var rentalPeriodId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var locationCityCenter = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var locationAirport = Guid.Parse("44444444-4444-4444-4444-444444444444");

        modelBuilder.Entity<Location>().HasData(
            new Location { Id = locationCityCenter, Name = "City Center" },
            new Location { Id = locationAirport, Name = "Airport" }
        );

        modelBuilder.Entity<Asset>().HasData(new Asset
        {
            Id = assetId,
            Code = "cityray",
            Name = "Geely Cityray",
            Type = AssetType.Auto,
            Category = AssetCategory.SUV,
            ShortDescription = "Краткое описание!",
            FullDescription = "Основные характеристики:",
            Status = AssetStatus.Available,
            DailyPrice = 6000,
            Deposit = 10000,
            LocationId = locationCityCenter,
            CanDeliver = true,
            DeliveryPrice = 500
        });

        modelBuilder.Entity<AssetPhoto>().HasData(new AssetPhoto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            AssetId = assetId,
            Url = "https://freeimage.host/i/fHys5hP"
        });

        modelBuilder.Entity<Booking>().OwnsOne(b => b.RentalPeriod);
        modelBuilder.Entity<Booking>().OwnsOne(b => b.AssetSnapshot);

        modelBuilder.Entity<RentalPeriod>().HasData(new RentalPeriod
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(3)
        });

        modelBuilder.Entity<Booking>().HasData(new Booking
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            CustomerId = customerId,
            TotalPrice = 18000,

        });
    }
}

