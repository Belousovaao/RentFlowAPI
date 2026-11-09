using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.BookingCharges.Handlers;

public sealed class ReviewBookingChargeHandler : IRequestHandler<ReviewBookingChargeCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IBookingChargeRepository _charges;
    private readonly IPaymentAllocationRepository _allocations;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    public ReviewBookingChargeHandler(IBookingRepository bookings, IBookingChargeRepository charges, IPaymentAllocationRepository allocations, IUnitOfWork uow, IDateTimeProvider clock)
    {
        _bookings = bookings;
        _charges = charges;
        _allocations = allocations;
        _uow = uow;
        _clock = clock;
    }

    public async Task Handle(ReviewBookingChargeCommand request, CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty || request.ChargeId == Guid.Empty)
            throw new ArgumentException("BookingId and ChargeId are required.");

        if (request.Confirm)
            throw new InvalidOperationException("Charges are accrued immediately when created; only rejection is supported.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
                ?? throw new KeyNotFoundException("Booking not found.");
            var charge = await _charges.GetByIdAsync(request.ChargeId, ct)
                ?? throw new KeyNotFoundException("Charge not found.");
            if (charge.BookingId != booking.Id)
                throw new InvalidOperationException("Charge does not belong to the specified booking.");

            // Отклонение начисления отменяет распределения на него.
            // Платежи сохраняются, поэтому суммы снова становятся доступными для распределения.
            await _allocations.DeleteByBookingChargeIdAsync(charge.Id, ct);
            charge.Reject(_clock.UtcNow);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
        }
        catch
        {
            await _uow.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}