using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Assets;

namespace RentFlow.Persistance.Configurations;

public sealed class AssetMaintenancePeriodConfiguration : IEntityTypeConfiguration<AssetMaintenancePeriod>
{
    public void Configure(EntityTypeBuilder<AssetMaintenancePeriod> builder)
    {
        builder.ToTable("AssetMaintenancePeriods", table =>
        {
            table.HasCheckConstraint("CK_AssetMaintenancePeriods_DateRange", "\"EndDate\" > \"StartDate\"");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasIndex(x => new { x.AssetId, x.StartDate, x.EndDate });
        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
