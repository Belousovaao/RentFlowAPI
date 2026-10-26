using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
namespace RentFlow.Persistance.Repositories;
public sealed class BookingChargeRepository : IBookingChargeRepository
{
    private readonly RentFlowDbContext _db;
    public BookingChargeRepository(RentFlowDbContext db) => _db = db;
    public async Task AddAsync(BookingCharge charge, CancellationToken ct = default) => await _db.BookingCharges.AddAsync(charge, ct);
    public Task<BookingCharge?> GetByIdAsync(Guid chargeId, CancellationToken ct = default) => _db.BookingCharges.FirstOrDefaultAsync(x => x.Id == chargeId, ct);
    public async Task<IReadOnlyList<BookingCharge>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await _db.BookingCharges.AsNoTracking().Where(x => x.BookingId == bookingId).OrderBy(x => x.CreatedAt).ToListAsync(ct);
    public Task<BookingChargeSettings?> GetSettingsAsync(CancellationToken ct = default) => _db.BookingChargeSettings.FirstOrDefaultAsync(x => x.Id == 1, ct);
    public async Task SaveSettingsAsync(BookingChargeSettings settings, CancellationToken ct = default)
    {
        if (_db.Entry(settings).State != EntityState.Detached) return;
        if (await _db.BookingChargeSettings.AnyAsync(x => x.Id == 1, ct)) _db.BookingChargeSettings.Update(settings);
        else await _db.BookingChargeSettings.AddAsync(settings, ct);
    }
}