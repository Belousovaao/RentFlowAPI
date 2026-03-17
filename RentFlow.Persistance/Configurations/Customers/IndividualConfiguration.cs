using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Configurations.Customer;

public class IndividualConfiguration : IEntityTypeConfiguration<Individual>
{
    public void Configure(EntityTypeBuilder<Individual> builder)
    {
        builder.OwnsOne(b => b.Name);
        builder.OwnsOne(b => b.IndividualPassport);
        builder.OwnsOne(b => b.IndividualDriverLicense);
        builder.OwnsOne(b => b.IndividualBankAccount);
    }

}
