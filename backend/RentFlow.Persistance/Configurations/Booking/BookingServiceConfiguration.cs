using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Bookings;

namespace RentFlow.Persistance.Configurations;

public class BookingServiceConfiguration: IEntityTypeConfiguration<BookingService>
{
    public void Configure(EntityTypeBuilder<BookingService> builder)
    {
        builder.ToTable("BookingServices");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.BookingId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.AdditionalServiceId)
            .IsRequired();

        builder.Property(x => x.SelectedTime);
    }
}