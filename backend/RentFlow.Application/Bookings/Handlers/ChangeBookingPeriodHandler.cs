using MediatR;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class ChangeBookingPeriodHandler: IRequestHandler<ChangeBookingPeriodCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookingPricingService _bookingPricingService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookingChargeRepository _bookingCharges;

    public ChangeBookingPeriodHandler(
        IBookingRepository bookings,
        IUnitOfWork unitOfWork,
        IBookingPricingService bookingPricingService,
        IDateTimeProvider dateTimeProvider,
        IBookingChargeRepository bookingCharges)
    {
        _bookings = bookings;
        _unitOfWork = unitOfWork;
        _bookingPricingService = bookingPricingService;
        _dateTimeProvider = dateTimeProvider;
        _bookingCharges = bookingCharges;
    }

    public async Task Handle(ChangeBookingPeriodCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);

        if (booking is null)
            throw new KeyNotFoundException("Booking not found.");

        if (booking.Status is not (
            BookingStatus.Pending or
            BookingStatus.Confirmed or
            BookingStatus.Active))
        {
            throw new InvalidOperationException("Booking period cannot be changed in the current status.");
        }

        var currentStartLocal = _dateTimeProvider.ToLocal(booking.RentalPeriod.StartDate);
        var currentEndLocal = _dateTimeProvider.ToLocal(booking.RentalPeriod.EndDate);
        var startUtc = _dateTimeProvider.ToUtc(request.StartDate, TimeOnly.FromDateTime(currentStartLocal));
        var endUtc = _dateTimeProvider.ToUtc(request.EndDate, TimeOnly.FromDateTime(currentEndLocal));
        var newPeriod = new RentalPeriod(startUtc, endUtc);
        var newRentalPrice =_bookingPricingService.CalculatePeriodChangePrice(
                booking.RentalPeriod,
                newPeriod,
                booking.Price.RentalPrice);
        BookingPenalty? penalty = null;
        if (booking.IsPeriodReduction(newPeriod) && !booking.HasPeriodReductionPenalty)
        {
            penalty = _bookingPricingService.CalculateReductionPenalty(booking.Price.RentalPrice);
        }
        booking.ChangePeriod(
            newPeriod,
            newRentalPrice,
            penalty);

        // Update the existing rental charge instead of recreating it, so payment
        // allocations remain attached to the same charge after a period change.
        var rentalCharges = (await _bookingCharges.GetTrackedByBookingIdAsync(booking.Id, ct))
            .Where(x => x.Type == BookingChargeType.Rental)
            .ToList();

        if (rentalCharges.Count > 1)
            throw new InvalidOperationException("Booking has more than one rental charge; finance data must be reconciled before changing the period.");

        var rentalAmount = booking.Price.RentalPrice.RentalPriceWithDiscount;
        if (rentalAmount <= 0)
            throw new InvalidOperationException("Rental price must be positive.");

        if (rentalCharges.Count == 1)
        {
            rentalCharges[0].UpdateInitialAmount(rentalAmount);
        }
        else
        {
            // Compatibility fallback for legacy bookings not covered by the backfill.
            // The backfill uses the booking start date because the true creation time is unavailable.
            await _bookingCharges.AddAsync(BookingCharge.Create(
                booking.Id, BookingChargeType.Rental, "Аренда", null, null,
                1m, "услуга", rentalAmount, rentalAmount, _dateTimeProvider.UtcNow), ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
