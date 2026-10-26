using System;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAdminListAsync(CancellationToken ct = default);
    Task<Booking?> GetAdminDetailsAsync(Guid bookingId, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<List<Booking>> GetOverlappingAsync(Guid assetId, RentalPeriod period, CancellationToken ct);
    Task<IReadOnlyList<Booking>> GetBookingsForAssetAsync(Guid assetId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetBookedAssetIdsAsync(IEnumerable<Guid> assetIds, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<bool> IsAssetAvailableAsync(Guid assetId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<List<BusyPeriodDto>> GetBusyPeriodsAsync(Guid assetId, Guid? excludeBookingId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<Customer?> GetCustomerByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    void ReplaceBookingServices(Booking booking, IEnumerable<BookingService> services);
}
