
using MediatR;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class ChangeBookingStatusHandler
    : IRequestHandler<ChangeBookingStatusCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeBookingStatusHandler(
        IBookingRepository bookings,
        IPaymentRepository payments,
        IUnitOfWork unitOfWork)
    {
        _bookings = bookings;
        _payments = payments;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ChangeBookingStatusCommand request,
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
                request.BookingId, ct)
                ?? throw new KeyNotFoundException("Booking not found.");

            switch (request.Status)
            {
                case BookingStatus.Cancelled:
                    await CancelWithoutPaymentAsync(
                        booking, ct);
                    break;

                case BookingStatus.Confirmed:
                {
                    var customer =
                        await _bookings.GetCustomerByBookingIdAsync(
                            request.BookingId, ct);

                    if (customer is null)
                        throw new InvalidOperationException(
                            "Booking customer not found.");

                    booking.Confirm(customer);
                    break;
                }

                case BookingStatus.Active:
                {
                    var customer =
                        await _bookings.GetCustomerByBookingIdAsync(
                            request.BookingId, ct);

                    if (customer is null)
                        throw new InvalidOperationException(
                            "Booking customer not found.");

                    booking.Activate(customer);
                    break;
                }

                case BookingStatus.Completed:
                    booking.Complete();
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Transition to status '{request.Status}' " +
                        "is not supported.");
            }

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

    private async Task CancelWithoutPaymentAsync(
        Booking booking,
        CancellationToken ct)
    {
        if (booking.Status is not (
            BookingStatus.Pending or BookingStatus.Confirmed))
        {
            throw new InvalidOperationException(
                "Only pending or confirmed bookings can be cancelled.");
        }

        var payments = await _payments.GetByBookingIdAsync(
            booking.Id, ct);

        var hasCompletedIncomingPayment = payments.Any(p =>
            (p.Type is PaymentType.Prepayment
                or PaymentType.AdditionalPayment) &&
            p.Status == PaymentStatus.Completed);

        if (hasCompletedIncomingPayment)
        {
            throw new InvalidOperationException(
                "The booking has received a payment. " +
                "Use CancelBookingCommand to calculate the refund.");
        }

        booking.Cancel();
    }
}
