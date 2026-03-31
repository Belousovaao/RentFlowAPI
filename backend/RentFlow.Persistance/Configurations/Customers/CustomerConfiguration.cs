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
    }
}
