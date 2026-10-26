using MediatR;
using RentFlow.Application.BookingCharges.Dtos;
using RentFlow.Application.BookingCharges.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class GetBookingChargesHandler : IRequestHandler<GetBookingChargesQuery, IReadOnlyList<BookingChargeDto>>
{
    private readonly IBookingRepository _bookings; private readonly IBookingChargeRepository _charges;
    private readonly IPaymentAllocationRepository _allocations; private readonly IPaymentRepository _payments;
    public GetBookingChargesHandler(IBookingRepository bookings, IBookingChargeRepository charges, IPaymentAllocationRepository allocations, IPaymentRepository payments)
    { _bookings = bookings; _charges = charges; _allocations = allocations; _payments = payments; }
    public async Task<IReadOnlyList<BookingChargeDto>> Handle(GetBookingChargesQuery request, CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty) throw new ArgumentException("BookingId is required.");
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct) ?? throw new KeyNotFoundException("Booking not found.");
        if (request.CustomerId.HasValue && booking.CustomerId != request.CustomerId.Value) throw new UnauthorizedAccessException("You cannot view charges for another customer's booking.");
        var charges = await _charges.GetByBookingIdAsync(request.BookingId, ct);
        var allocations = await _allocations.GetByBookingIdAsync(request.BookingId, ct);
        var payments = await _payments.GetByBookingIdAsync(request.BookingId, ct);
        var completedIds = payments.Where(p => p.Status == PaymentStatus.Completed && p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment).Select(p => p.Id).ToHashSet();
        return charges.Select(c => {
            var paid = c.Status == BookingChargeStatus.Confirmed ? allocations.Where(a => a.BookingChargeId == c.Id && completedIds.Contains(a.PaymentId)).Sum(a => a.Amount) : 0m;
            return new BookingChargeDto(c.Id,c.BookingId,c.Type,c.Status,c.Title,c.Description,c.ReferenceNumber,c.Quantity,c.Unit,c.UnitPrice,c.Amount,c.Currency,paid,Math.Max(0m,c.Amount-paid),c.CreatedAt,c.ReviewedAt);
        }).ToList();
    }
}