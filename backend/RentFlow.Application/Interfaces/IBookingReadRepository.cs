using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Interfaces;

public interface IBookingReadRepository
{
    Task<IReadOnlyList<AdminBookingListItemDto>> GetAdminBookingsAsync(CancellationToken ct = default);

    Task<AdminBookingDetailsDto?> GetAdminBookingDetailsAsync( Guid bookingId, CancellationToken ct = default);

    Task<IReadOnlyList<BookingListItemDto>> GetCustomerBookingsAsync(Guid customerId, CancellationToken ct = default);
    Task<BookingDetailsDto?> GetCustomerBookingDetailsAsync(Guid customerId, Guid bookingId, CancellationToken ct = default);
}