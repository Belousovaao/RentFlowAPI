using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Locations;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Persistance;

public class RentFlowDbContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<AssetPhoto> AssetPhotos => Set<AssetPhoto>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<PricingRule> PricingRules => Set<PricingRule>();
    public DbSet<Customer> Customers => Set<Customer>();


    public RentFlowDbContext(DbContextOptions<RentFlowDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentFlowDbContext).Assembly);
    }
}

