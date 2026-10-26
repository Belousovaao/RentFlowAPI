using System;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Customers;

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
    }
    public async Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Services)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.RentalPeriod.StartDate)
            .ToListAsync(ct);
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Bookings.Include(b => b.Services).FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<Booking>> GetAdminListAsync(CancellationToken ct = default)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Services)
            .OrderBy(b => b.Status == BookingStatus.Pending ? 0 : 1)
            .ThenByDescending(b => b.RentalPeriod.StartDate)
            .ToListAsync(ct);
    }

    public async Task<Booking?> GetAdminDetailsAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Services)
            .Include(b => b.Roles)
            .FirstOrDefaultAsync(b => b.Id == bookingId, ct);
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

    public async Task<List<BusyPeriodDto>> GetBusyPeriodsAsync(Guid assetId, Guid? excludeBookingId, CancellationToken ct)
    {
        return await _db.Bookings
            .Where(x =>
                x.AssetId == assetId &&
                x.Status != BookingStatus.Cancelled &&
                x.Status != BookingStatus.Completed &&
                (excludeBookingId == null || x.Id != excludeBookingId))
            .OrderBy(x => x.RentalPeriod.StartDate)
            .Select(x => new BusyPeriodDto(
                x.RentalPeriod.StartDate,
                x.RentalPeriod.EndDate))
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Customer?> GetCustomerByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        var customerId = await _db.Bookings
            .Where(b => b.Id == bookingId)
            .Select(b => b.CustomerId)
            .FirstOrDefaultAsync(ct);

        if (customerId == Guid.Empty)
            return null;

        return await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId, ct);
    }

    public void ReplaceBookingServices(Booking booking, IEnumerable<BookingService> services)
    {
        _db.BookingServices.RemoveRange(booking.Services);
        foreach (var service in services)
        {
            service.SetBookingId(booking.Id);
        }
        _db.BookingServices.AddRange(services);
    }
}
