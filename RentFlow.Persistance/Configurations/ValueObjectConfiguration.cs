using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Configurations;

public static class ValueObjectConfiguration
{
    public static void ConfigurePersonName<TEntity>(this OwnedNavigationBuilder<TEntity, PersonName> builder) where TEntity : class
    {
        builder.Property(p => p.FirstName);
        builder.Property(p => p.LastName);
        builder.Property(p => p.MiddleName);
    }

    public static void ConfigureDriverLicense<TEntity>(this OwnedNavigationBuilder<TEntity, DriverLicense> builder) where TEntity : class
    {
        builder.Property(p => p.Number);
        builder.Property(p => p.IssuedDate);
        builder.Property(p => p.ExpirationDate);
        builder.Property(p => p.Categories).HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c[0]).ToList());
    }

    public static void ConfigureBankAccount<TEntity>(this OwnedNavigationBuilder<TEntity, BankAccount> builder) where TEntity : class
    {
        builder.Property(p => p.BankName);
        builder.Property(p => p.BIK);
        builder.Property(p => p.CorrespondentAccount);
        builder.Property(p => p.CurrentAccount);
    }

    public static void ConfigurePassport<TEntity>(this OwnedNavigationBuilder<TEntity, Passport> builder) where TEntity : class
    {
        builder.Property(p => p.Serial);
        builder.Property(p => p.Number);
        builder.Property(p => p.IssuedBy);
        builder.Property(p => p.IssuedDate);
        builder.Property(p => p.RegistrationAddress);
    }
}
