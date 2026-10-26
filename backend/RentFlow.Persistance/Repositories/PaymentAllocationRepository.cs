using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Payments;
namespace RentFlow.Persistance.Repositories;
public sealed class PaymentAllocationRepository : IPaymentAllocationRepository
{
    private readonly RentFlowDbContext _db;
    public PaymentAllocationRepository(RentFlowDbContext db) => _db = db;
    public async Task AddRangeAsync(IEnumerable<PaymentAllocation> allocations, CancellationToken ct = default) => await _db.PaymentAllocations.AddRangeAsync(allocations, ct);
    public async Task<IReadOnlyList<PaymentAllocation>> GetByPaymentIdAsync(Guid paymentId, CancellationToken ct = default)
        => await _db.PaymentAllocations.AsNoTracking().Where(x => x.PaymentId == paymentId).ToListAsync(ct);
    public async Task<IReadOnlyList<PaymentAllocation>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await (from allocation in _db.PaymentAllocations.AsNoTracking()
                  join charge in _db.BookingCharges.AsNoTracking() on allocation.BookingChargeId equals charge.Id
                  where charge.BookingId == bookingId select allocation).ToListAsync(ct);
}