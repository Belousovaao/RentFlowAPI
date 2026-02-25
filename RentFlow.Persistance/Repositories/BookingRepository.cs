using System;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;

namespace RentFlow.Persistance.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly RentFlowDbContext _db;
    public BookingRepository(RentFlowDbContext db)
    {
        _db = db;
    }
    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
        await _db.Bookings.AddAsync(booking);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return  _db.Bookings.AsNoTracking().ToList();
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<Booking>> GetOverlappingAsync(Guid assetId, RentalPeriod period, CancellationToken ct)
    {
        return await _db.Bookings.Where(b =>
            b.AssetId == assetId &&
            b.Status != BookingStatus.Cancelled &&
            b.Status != BookingStatus.Completed &&
            b.RentalPeriod.StartDate < period.EndDate &&
            b.RentalPeriod.EndDate > period.StartDate)
            .ToListAsync(ct);
    }
}
