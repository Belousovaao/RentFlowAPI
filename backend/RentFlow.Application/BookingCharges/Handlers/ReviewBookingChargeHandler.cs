using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.Interfaces;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class ReviewBookingChargeHandler : IRequestHandler<ReviewBookingChargeCommand>
{
    private readonly IBookingRepository _bookings; private readonly IBookingChargeRepository _charges; private readonly IUnitOfWork _uow; private readonly IDateTimeProvider _clock;
    public ReviewBookingChargeHandler(IBookingRepository bookings, IBookingChargeRepository charges, IUnitOfWork uow, IDateTimeProvider clock) { _bookings = bookings; _charges = charges; _uow = uow; _clock = clock; }
    public async Task Handle(ReviewBookingChargeCommand request, CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty || request.ChargeId == Guid.Empty) throw new ArgumentException("BookingId and ChargeId are required.");
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var booking = await _bookings.GetByIdAsync(request.BookingId, ct) ?? throw new KeyNotFoundException("Booking not found.");
            var charge = await _charges.GetByIdAsync(request.ChargeId, ct) ?? throw new KeyNotFoundException("Charge not found.");
            if (charge.BookingId != booking.Id) throw new InvalidOperationException("Charge does not belong to the specified booking.");
            if (request.Confirm) charge.Confirm(_clock.UtcNow); else charge.Reject(_clock.UtcNow);
            await _uow.SaveChangesAsync(ct); await _uow.CommitAsync(ct);
        }
        catch { await _uow.RollbackAsync(CancellationToken.None); throw; }
    }
}