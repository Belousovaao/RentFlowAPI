using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Bookings;

namespace RentFlow.Persistance.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.OwnsOne(b => b.RentalPeriod, rp =>
        {
            rp.Property(p => p.StartDate).HasColumnName("StartDate").IsRequired();
            rp.Property(p => p.EndDate).HasColumnName("EndDate").IsRequired();
        });
        builder.OwnsOne(b => b.AssetSnapshot);

        builder.OwnsOne(b => b.CustomerSnapshot, cs =>
        {
            cs.Property(p => p.Type);

            cs.OwnsOne(p => p.Name, n =>
            {
                n.ConfigurePersonName();
            });

            cs.OwnsOne(p => p.Passport, p =>
            {
                p.ConfigurePassport();
            });

            cs.OwnsOne(p => p.BankAccount, b =>
            {
                b.ConfigureBankAccount();
            });

            cs.Property(p => p.Email);
            cs.Property(p => p.Phone);

            cs.Property(p => p.OrganizationName);
            cs.Property(p => p.ShortName);
            cs.Property(p => p.KPP);

            cs.Property(p => p.OrganizationAddress);
            cs.Property(p => p.FactAddress);
        });
        builder.Navigation(b => b.CustomerSnapshot).IsRequired();


        builder.OwnsOne(b => b.BookingDriver, driver =>
        {
            driver.OwnsOne(d => d.Name, name =>
            {
                name.ConfigurePersonName();

                name.Property(p => p.FirstName).IsRequired(false);
                name.Property(p => p.LastName).IsRequired(false);
                name.Property(p => p.MiddleName).IsRequired(false);
            });

            driver.OwnsOne(d => d.License, license => 
            {
                license.ConfigureDriverLicense();

                license.Property(p => p.Number).IsRequired(false);
                license.Property(p => p.IssuedDate).IsRequired(false);
                license.Property(p => p.ExpirationDate).IsRequired(false);
                license.Property(p => p.Categories).IsRequired(false);
            });

            driver.Property(d => d.Phone).IsRequired(false);

            // 1. Связь с коллекцией Roles (один-ко-многим)
        builder.HasMany(b => b.Roles)
            .WithOne()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. Связь с Signatory (один-к-одному через отдельное поле)
        builder.HasOne(b => b.Signatory)
            .WithOne()
            .HasForeignKey<Booking>(b => b.SignatoryId)
            .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
