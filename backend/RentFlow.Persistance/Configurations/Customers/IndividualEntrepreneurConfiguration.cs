using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Configurations.Customer;

public class IndividualEntrepreneurConfiguration : IEntityTypeConfiguration<IndividualEntrepreneur>
{
    public void Configure(EntityTypeBuilder<IndividualEntrepreneur> builder)
    {
        builder.OwnsOne(b => b.Name);
        builder.OwnsOne(b => b.IPBankAccount);
        builder.OwnsOne(b => b.IPPassport);
        builder.OwnsOne(b => b.EntrepreneurDriverLicense);
        builder.OwnsOne(x => x.IpLegalForm, lf =>
            {
                lf.Property(p => p.ShortName).HasColumnName("LegalFormShortName");
                lf.Property(p => p.FullName).HasColumnName("LegalFormFullName");
            });
    }

}
