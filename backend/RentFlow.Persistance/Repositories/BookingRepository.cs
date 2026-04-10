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

    public async Task<IReadOnlyList<Guid>> GetBookedAssetIdsAsync(
        IEnumerable<Guid> assetIds, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken ct = default)
    {
        // Получаем бронирования, которые пересекаются с указанным периодом
        var bookings = await _db.Bookings
            .Where(b => assetIds.Contains(b.AssetId))
            .Where(b => b.Status != BookingStatus.Cancelled) // Исключаем отмененные
            .Where(b => b.RentalPeriod.StartDate < endDate && b.RentalPeriod.EndDate > startDate) // Пересечение периодов
            .Select(b => b.AssetId)
            .Distinct()
            .ToListAsync(ct);

        return bookings;
    }

    public async Task<bool> IsAssetAvailableAsync(
        Guid assetId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken ct = default)
    {
        // Проверяем, есть ли активные бронирования на этот период
        var hasConflictingBooking = await _db.Bookings
            .AnyAsync(b => 
                b.AssetId == assetId &&
                b.Status != BookingStatus.Cancelled &&
                b.RentalPeriod.StartDate < endDate && 
                b.RentalPeriod.EndDate > startDate,
                ct);

        return !hasConflictingBooking;
    }

    public async Task<IReadOnlyList<Booking>> GetBookingsForAssetAsync(
        Guid assetId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken ct = default)
    {
        return await _db.Bookings
            .Where(b => b.AssetId == assetId)
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Where(b => b.RentalPeriod.StartDate < endDate && b.RentalPeriod.EndDate > startDate)
            .ToListAsync(ct);
    }
}
