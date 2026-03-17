using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Assets;

namespace RentFlow.Persistance.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasMany(a => a.Photos).WithOne()
            .HasForeignKey(p => p.AssetId).OnDelete(DeleteBehavior.Cascade);
    }
}
