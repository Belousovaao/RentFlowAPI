using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Configurations.Customer;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.OwnsOne(b => b.SigningBasis);
        builder.OwnsOne(b => b.OrganizationBankAccount);
        builder.OwnsOne(x => x.LegalForm, lf =>
            {
                lf.Property(p => p.ShortName).HasColumnName("LegalFormShortName");
                lf.Property(p => p.FullName).HasColumnName("LegalFormFullName");
            });
    }

}
