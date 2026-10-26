
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Payments;
using RentFlow.Persistance;

namespace RentFlow.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly RentFlowDbContext _db;

    public PaymentRepository(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken ct = default)
    {
        await _db.Payments.AddAsync(payment, ct);
    }

    public async Task<IReadOnlyList<Payment>> GetByBookingIdAsync(
        Guid bookingId,
        CancellationToken ct = default)
    {
        return await _db.Payments
            .AsNoTracking()
            .Where(p => p.BookingId == bookingId)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Payment?> GetByIdAsync(
        Guid paymentId,
        CancellationToken ct = default)
    {
        return await _db.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId, ct);
    }

    public async Task<Guid?> GetBookingIdByPaymentIdAsync(
        Guid paymentId,
        CancellationToken ct = default)
    {
        return await _db.Payments
            .AsNoTracking()
            .Where(p => p.Id == paymentId)
            .Select(p => (Guid?)p.BookingId)
            .SingleOrDefaultAsync(ct);
    }
}