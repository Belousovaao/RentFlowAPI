using RentFlow.Domain.Payments;
namespace RentFlow.Application.Interfaces;
public interface IPaymentAllocationRepository
{
    Task AddRangeAsync(IEnumerable<PaymentAllocation> allocations, CancellationToken ct = default);
    Task<IReadOnlyList<PaymentAllocation>> GetByPaymentIdAsync(Guid paymentId, CancellationToken ct = default);
    Task<IReadOnlyList<PaymentAllocation>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
}