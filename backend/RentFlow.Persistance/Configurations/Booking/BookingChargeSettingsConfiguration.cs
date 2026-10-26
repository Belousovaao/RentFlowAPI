using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Bookings;
namespace RentFlow.Persistance.Configurations;
public sealed class BookingChargeSettingsConfiguration : IEntityTypeConfiguration<BookingChargeSettings>
{
    public void Configure(EntityTypeBuilder<BookingChargeSettings> builder)
    {
        builder.ToTable("BookingChargeSettings", table => table.HasCheckConstraint("CK_BookingChargeSettings_Singleton", "\"Id\" = 1"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LateCancellationTariff).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.RefuelingPricePerLiter).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}