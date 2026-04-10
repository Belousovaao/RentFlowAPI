using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Assets;

namespace RentFlow.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(a => a.Code)
            .IsUnique();
            
        builder.Property(a => a.BrandName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.Model)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.Year);
        builder.Property(a => a.Seats);
        builder.Property(a => a.Doors);
        builder.Property(a => a.Horsepower);
        builder.Property(a => a.TopSpeed);
        builder.Property(a => a.Color).HasMaxLength(50);
        
        builder.Property(a => a.FuelType)
            .HasConversion<int>();
            
        builder.Property(a => a.Transmission)
            .HasConversion<int>();

        builder.Property(a => a.DriveType).HasConversion<int>();
            
        builder.Property(a => a.Status)
            .HasConversion<int>();
            
        builder.Property(a => a.Features)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
            
        builder.OwnsMany(a => a.Photos, photo =>
        {
            photo.WithOwner().HasForeignKey("AssetId");
            photo.Property(p => p.Url).IsRequired().HasMaxLength(500);
            photo.HasKey(p => p.Id);
        });
        
        builder.HasIndex(a => a.LocationId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.DailyPrice);
    }
}