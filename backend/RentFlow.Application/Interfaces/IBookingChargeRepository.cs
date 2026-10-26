using RentFlow.Domain.Bookings;
namespace RentFlow.Application.Interfaces;
public interface IBookingChargeRepository
{
    Task AddAsync(BookingCharge charge, CancellationToken ct = default);
    Task<BookingCharge?> GetByIdAsync(Guid chargeId, CancellationToken ct = default);
    Task<IReadOnlyList<BookingCharge>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<BookingChargeSettings?> GetSettingsAsync(CancellationToken ct = default);
    Task SaveSettingsAsync(BookingChargeSettings settings, CancellationToken ct = default);
}