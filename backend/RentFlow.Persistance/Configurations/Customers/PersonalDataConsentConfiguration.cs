using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Configurations.Customers;

public sealed class PersonalDataConsentConfiguration : IEntityTypeConfiguration<PersonalDataConsent>
{
    public void Configure(EntityTypeBuilder<PersonalDataConsent> builder)
    {
        builder.ToTable("PersonalDataConsents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GrantedAt)
            .IsRequired();

        builder.Property(x => x.Method)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.ConsentDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CustomerId,
            x.GrantedAt
        });
    }
}