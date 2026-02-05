using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Locations;

namespace RentFlow.Persistance;

public class RentFlowDbContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<AssetPhoto> AssetPhotos => Set<AssetPhoto>();

    public RentFlowDbContext(DbContextOptions<RentFlowDbContext> options)
        : base(options) { }
}

