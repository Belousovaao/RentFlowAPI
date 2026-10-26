using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;
using CustomerEntity = RentFlow.Domain.Customers.Customer;

namespace RentFlow.Persistance.Configurations.Customers;

public class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder
            .HasDiscriminator<CustomerType>("Type")
            .HasValue<Individual>(CustomerType.Individual)
            .HasValue<IndividualEntrepreneur>(CustomerType.IndividualEntrepreneur)
            .HasValue<Organization>(CustomerType.Organization);

        builder
            .HasMany(c => c.Documents)
            .WithOne(d => d.Customer)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(c => c.Documents)
            .HasField("_documents")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(c => c.PersonalDataConsents)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Navigation(c => c.PersonalDataConsents)
            .HasField("_personalDataConsents")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
