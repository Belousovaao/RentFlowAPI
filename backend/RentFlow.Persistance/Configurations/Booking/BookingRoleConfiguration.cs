using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Bookings;

namespace RentFlow.Persistance.Configurations;

public class BookingRoleConfiguration : IEntityTypeConfiguration<BookingRole>
{
    public void Configure(EntityTypeBuilder<BookingRole> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.BookingId)
            .IsRequired();
        
        builder.Property(r => r.PersonId)
            .IsRequired();
        
        builder.Property(r => r.RoleType)
            .HasConversion<int>();
        
        // Настройка индекса для поиска по бронированию
        builder.HasIndex(r => r.BookingId);
    }
}