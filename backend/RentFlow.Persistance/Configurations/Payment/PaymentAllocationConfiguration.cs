using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Payments;
namespace RentFlow.Persistance.Configurations;
public sealed class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
    {
        builder.ToTable("PaymentAllocations"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne<Payment>().WithMany().HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<RentFlow.Domain.Bookings.BookingCharge>().WithMany().HasForeignKey(x => x.BookingChargeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.PaymentId);
        builder.HasIndex(x => x.BookingChargeId);
        builder.HasIndex(x => new { x.PaymentId, x.BookingChargeId }).IsUnique();
    }
}