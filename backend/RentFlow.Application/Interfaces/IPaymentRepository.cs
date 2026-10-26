
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);

    Task<IReadOnlyList<Payment>> GetByBookingIdAsync(
        Guid bookingId,
        CancellationToken ct = default);

    Task<Payment?> GetByIdAsync(
        Guid paymentId,
        CancellationToken ct = default);

    Task<Guid?> GetBookingIdByPaymentIdAsync(
        Guid paymentId,
        CancellationToken ct = default);
}
