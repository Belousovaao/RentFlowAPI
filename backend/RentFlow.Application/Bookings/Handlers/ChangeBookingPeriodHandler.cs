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

    public ChangeBookingPeriodHandler(
        IBookingRepository bookings,
        IUnitOfWork unitOfWork,
        IBookingPricingService bookingPricingService,
        IDateTimeProvider dateTimeProvider)
    {
        _bookings = bookings;
        _unitOfWork = unitOfWork;
        _bookingPricingService = bookingPricingService;
        _dateTimeProvider = dateTimeProvider;
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

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
