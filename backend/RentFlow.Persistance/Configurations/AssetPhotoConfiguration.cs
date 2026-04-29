using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Assets;

namespace RentFlow.Infrastructure.Persistence.Configurations;

public class AssetPhotoConfiguration : IEntityTypeConfiguration<AssetPhoto>
{
    public void Configure(EntityTypeBuilder<AssetPhoto> builder)
    {
        builder.ToTable("AssetPhotos");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(p => p.Type)
            .HasConversion<int>();
        
        builder.Property(p => p.Order);
        
        builder.HasOne<Asset>()
            .WithMany(a => a.Photos)
            .HasForeignKey(p => p.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}