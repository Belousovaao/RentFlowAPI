using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;

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

        builder.OwnsOne(b => b.Destination, destination =>
        {
            destination.Property(d => d.DestinationId)
                .HasColumnName("DestinationId")
                .IsRequired(false);

            destination.Property(d => d.Type)
                .HasColumnName("DestinationType")
                .HasConversion<string>()
                .IsRequired();

            destination.Property(d => d.CustomRouteDescription)
                .HasColumnName("CustomRouteDescription")
                .HasMaxLength(2000)
                .IsRequired(false);

            destination.Property(d => d.ReviewStatus)
                .HasColumnName("RouteReviewStatus")
                .HasConversion<string>()
                .IsRequired();

            destination.Property(d => d.DestinationNameSnapshot)
                .HasColumnName("DestinationNameSnapshot")
                .HasMaxLength(200)
                .IsRequired(false);
        });

        builder.Navigation(b => b.Destination).IsRequired();

        builder.OwnsOne(b => b.AssetSnapshot);

        builder.OwnsOne(b => b.Price, price =>
        {
            price.OwnsOne(p => p.RentalPrice, rentalPrice =>
            {
                rentalPrice.OwnsMany(p => p.Segments, segment =>
                {
                    segment.ToTable("BookingRentalPriceSegments");
                    segment.HasKey(x => x.Id);
                    segment.Property(x => x.Id).ValueGeneratedNever();
                    segment.Property(p => p.Order).HasColumnName("Order").IsRequired();
                    segment.Property(p => p.Days).HasColumnName("Days").IsRequired();
                    segment.Property(p => p.BaseDailyPrice).HasColumnName("BaseDailyPrice").HasPrecision(18, 2).IsRequired();
                    segment.Property(p => p.DurationDiscountPercent).HasColumnName("DurationDiscountPercent").HasPrecision(5, 2).IsRequired();
                    segment.Property(p => p.DiscountedDailyPrice).HasColumnName("DiscountedDailyPrice").HasPrecision(18, 2).IsRequired();
                    segment.Property(p => p.BaseRentalPrice).HasColumnName("BaseRentalPrice").HasPrecision(18, 2).IsRequired();
                    segment.Property(p => p.DiscountAmount).HasColumnName("DiscountAmount").HasPrecision(18, 2).IsRequired();
                    segment.Property(p => p.Total).HasColumnName("Total").HasPrecision(18, 2).IsRequired();
                    segment.Property(p => p.AppliedForDays).HasColumnName("AppliedForDays").IsRequired();
                });
            });
            price.Property(p => p.Deposit).HasColumnName("Deposit").HasPrecision(18, 2);
            price.Property(p => p.ServicesPrice).HasColumnName("ServicesPrice").HasPrecision(18, 2);
            price.OwnsMany(p => p.Penalties, penalty =>
            {
                penalty.ToTable("BookingPenalties");
                penalty.HasKey(x => x.Id);
                penalty.Property(x => x.Id).ValueGeneratedNever();
                penalty.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();
                penalty.Property(x => x.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
            });
            price.Property(p => p.RouteSurcharge)
                .HasColumnName("RouteSurcharge")
                .HasPrecision(18, 2)
                .IsRequired();
        });

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
                license.Property(p => p.ExprerienceFromYear).IsRequired(false);
                license.Property(p => p.Categories).IsRequired(false);
            });

            driver.Property(d => d.Phone).IsRequired(false);
        });

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

        builder.HasMany<BookingService>(b => b.Services)
            .WithOne()
            .HasForeignKey(s => s.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
