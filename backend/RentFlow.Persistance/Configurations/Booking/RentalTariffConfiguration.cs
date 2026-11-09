using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Settings;

namespace RentFlow.Persistance.Configurations;

public sealed class RentalTariffConfiguration : IEntityTypeConfiguration<RentalTariff>
{
    public void Configure(EntityTypeBuilder<RentalTariff> builder)
    {
        builder.ToTable("RentalTariffs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_RentalTariffs_Price_NonNegative",
            "\"Price\" >= 0"));

        builder.Property(x => x.Unit)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc).IsRequired();
    }
}
