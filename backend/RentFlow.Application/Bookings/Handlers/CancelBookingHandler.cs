
using MediatR;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class CancelBookingHandler : IRequestHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelBookingHandler(
        IBookingRepository bookings,
        IPaymentRepository payments,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _bookings = bookings;
        _payments = payments;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        CancelBookingCommand request,
        CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty)
            throw new ArgumentException("BookingId is required.");

        await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await _unitOfWork.LockBookingPaymentsAsync(
                request.BookingId, ct);

            var booking = await _bookings.GetByIdAsync(
                request.BookingId, ct);

            if (booking is null)
                throw new KeyNotFoundException("Booking not found.");

            if (request.CustomerId.HasValue && booking.CustomerId != request.CustomerId.Value)
            {
                throw new UnauthorizedAccessException("You cannot cancel another customer's booking.");
            }

            if (booking.Status is not (
                BookingStatus.Pending or BookingStatus.Confirmed))
            {
                throw new InvalidOperationException("Only pending or confirmed bookings can be cancelled.");
            }

            var now = _dateTimeProvider.UtcNow;
             // StartDate хранится как UTC-момент времени.
            // Не выполняем повторное преобразование через часовой пояс города.
            var startDate = booking.RentalPeriod.StartDate;

            Console.WriteLine($"StartDate={startDate:O}; Kind={startDate.Kind}");

            var rentalStart = new DateTimeOffset(startDate);

            if (rentalStart <= now)
            {
                throw new InvalidOperationException("A booking cannot be cancelled after the rental has started.");
            }

            var payments = await _payments.GetByBookingIdAsync(
                booking.Id, ct);

            var incomingPayments = payments
                .Where(p =>
                    p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment)
                .ToList();
            var completedIncomingPayments = incomingPayments
                .Where(p => p.Status == PaymentStatus.Completed)
                .ToList();

            if (booking.Status == BookingStatus.Pending && completedIncomingPayments.Count > 0)
            {
                throw new InvalidOperationException("A pending booking has a completed incoming payment. " + "Resolve the payment before cancelling.");
            }

            // Отменяем незавершённые входящие платежи, чтобы они
            // не оставались ожидающими после отмены бронирования.
            foreach (var payment in incomingPayments.Where(
                         p => p.Status == PaymentStatus.Pending))
            {
                // Для изменения статуса Payment должен быть отслеживаемым.
                // Если репозиторий возвращает AsNoTracking, см. примечание ниже.
                var trackedPayment = await _payments.GetByIdAsync(payment.Id, ct);

                if (trackedPayment?.Status == PaymentStatus.Pending)
                    trackedPayment.Cancel();
            }

            if (booking.Status == BookingStatus.Confirmed)
            {
                var reservedRefunds = payments
                    .Where(p =>
                        p.Type == PaymentType.Refund &&
                        p.OriginalPaymentId.HasValue &&
                        p.Status is PaymentStatus.Pending
                            or PaymentStatus.Completed)
                    .GroupBy(p => p.OriginalPaymentId!.Value)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Sum(p => p.Amount));
                
                var availableByPayment = completedIncomingPayments
                    .Select(payment => new
                    {
                        Payment = payment,
                        Available = Math.Max(
                            0m,
                            payment.Amount -
                            reservedRefunds.GetValueOrDefault(payment.Id))
                    })
                    .Where(x => x.Available > 0m)
                    .OrderBy(x => x.Payment.CreatedAt)
                    .ToList();

                var totalAvailable = availableByPayment.Sum(
                    x => x.Available);

                // До начала аренды осталось не менее 24 часов:
                // удержание не применяется.
                var retainedAmount = 0m;

                if (rentalStart - now < TimeSpan.FromHours(24) && totalAvailable > 0m)
                {
                    var firstSegment = booking.Price.RentalPrice.Segments
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException("The rental price has no segments.");

                    retainedAmount = Math.Min(
                        firstSegment.DiscountedDailyPrice,
                        totalAvailable);
                }

                var refundAmount = Math.Max(
                    0m,
                    totalAvailable - retainedAmount);

                // Распределяем возврат по исходным платежам
                // в порядке их создания. Каждый возврат ссылается
                // на конкретный входящий платёж.
                foreach (var item in availableByPayment)
                {
                    if (refundAmount <= 0m)
                        break;

                    var amountToRefund = Math.Min(
                        item.Available,
                        refundAmount);
                    
                    var refund = Payment.CreateRefund(
                        bookingId: booking.Id,
                        originalPaymentId: item.Payment.Id,
                        amount: amountToRefund,
                        method: item.Payment.Method,
                        createdAt: now,
                        note: request.Reason);

                    await _payments.AddAsync(refund, ct);

                    refundAmount -= amountToRefund;
                }

                if (refundAmount > 0m)
                {
                    throw new InvalidOperationException(
                        "The refund amount could not be fully allocated " +
                        "to the original payments.");
                }
            }

            booking.Cancel();

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(
                CancellationToken.None);
            throw;
        }
    }
}
