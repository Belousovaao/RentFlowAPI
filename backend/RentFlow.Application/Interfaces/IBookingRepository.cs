using System;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<List<Booking>> GetOverlappingAsync(Guid assetId, RentalPeriod period, CancellationToken ct);
}
