using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Auth;

namespace RentFlow.Persistance.Configurations.Auth;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email)
            .HasDatabaseName("UX_AppUsers_Email")
            .IsUnique()
            .HasFilter("\"Email\" IS NOT NULL");
        builder.HasIndex(u => u.Phone)
            .HasDatabaseName("UX_AppUsers_Phone")
            .IsUnique()
            .HasFilter("\"Phone\" IS NOT NULL");
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.Phone).HasMaxLength(32);
        builder.Property(u => u.FirstName).HasMaxLength(128);
        builder.Property(u => u.LastName).HasMaxLength(128);
        builder.Property(u => u.Role).HasMaxLength(32);
    }
}
