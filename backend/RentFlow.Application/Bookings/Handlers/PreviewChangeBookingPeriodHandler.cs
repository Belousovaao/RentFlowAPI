using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Queries;

public sealed class PreviewChangeBookingPeriodHandler: IRequestHandler<PreviewChangeBookingPeriodQuery, ChangeBookingPeriodPreviewDto>
{
    private readonly IBookingRepository _bookings;
    private readonly IBookingPricingService _bookingPricingService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PreviewChangeBookingPeriodHandler(
        IBookingRepository bookings,
        IBookingPricingService bookingPricingService,
        IDateTimeProvider dateTimeProvider)
    {
        _bookings = bookings;
        _bookingPricingService = bookingPricingService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ChangeBookingPeriodPreviewDto> Handle(PreviewChangeBookingPeriodQuery request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct);

        if (booking is null)
            throw new KeyNotFoundException("Бронирование не найдено.");

        if (request.CustomerId.HasValue && booking.CustomerId != request.CustomerId.Value)
            throw new UnauthorizedAccessException("Нет доступа к этому бронированию.");

        if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed or BookingStatus.Active))
            throw new InvalidOperationException("Срок аренды нельзя изменить в текущем статусе бронирования.");

        var currentStartLocal = _dateTimeProvider.ToLocal(booking.RentalPeriod.StartDate);
        var currentEndLocal = _dateTimeProvider.ToLocal(booking.RentalPeriod.EndDate);
        var startUtc = _dateTimeProvider.ToUtc(request.StartDate, TimeOnly.FromDateTime(currentStartLocal));
        var endUtc = _dateTimeProvider.ToUtc(request.EndDate, TimeOnly.FromDateTime(currentEndLocal));
        var newPeriod = new RentalPeriod(startUtc, endUtc);
        var newRentalPrice = _bookingPricingService.CalculatePeriodChangePrice(
            booking.RentalPeriod,
            newPeriod,
            booking.Price.RentalPrice);
        var currentRentalPrice = booking.Price.RentalPrice.RentalPriceWithDiscount;
        var newRentalPriceValue = newRentalPrice.RentalPriceWithDiscount;
        var currentTotal = booking.Price.Total;
        var currentRentalDays = GetRentalDays(booking.RentalPeriod);
        var actualNewRentalDays = GetRentalDays(newPeriod);
        var isExtension = actualNewRentalDays > currentRentalDays;
        var isReduction = actualNewRentalDays < currentRentalDays;
        var penalty = isReduction &&
                    booking.Status == BookingStatus.Active &&
                    !booking.HasPeriodReductionPenalty
            ? _bookingPricingService.CalculateReductionPenalty(booking.Price.RentalPrice).Amount
            : 0m;
        var newTotal = newRentalPriceValue + booking.Price.ServicesPrice + booking.Price.RouteSurcharge + penalty;
        var difference = newTotal - currentTotal;
        var currentSegments = booking.Price.RentalPrice.Segments
            .Select(MapSegment)
            .ToList();

        var changedSegments = isExtension
            ? newRentalPrice.Segments
                .Skip(booking.Price.RentalPrice.Segments.Count)
                .Select(MapSegment)
                .ToList()
            : newRentalPrice.Segments
                .Select(MapSegment)
                .ToList();

        return new ChangeBookingPeriodPreviewDto(
            StartDate: newPeriod.StartDate,
            EndDate: newPeriod.EndDate,
            CurrentRentalDays: currentRentalDays,
            NewRentalDays: actualNewRentalDays,
            CurrentDailyPrice: booking.Price.RentalPrice.LastSegment.DiscountedDailyPrice,
            NewDailyPrice: newRentalPrice.LastSegment.DiscountedDailyPrice,
            ChangedDays: actualNewRentalDays - currentRentalDays,
            CurrentRentalPrice: currentRentalPrice,
            NewRentalPrice: newRentalPriceValue,
            Penalty: penalty,
            CurrentTotal: currentTotal,
            NewTotal: newTotal,
            Difference: difference,
            IsExtension: isExtension,
            IsReduction: isReduction,
            IsAdditionalPayment: difference > 0,
            IsRefund: difference < 0,
            CurrentSegments: currentSegments,
            ChangedSegments: changedSegments);
    }

    private static int GetRentalDays(RentalPeriod period)
    {
        return (period.EndDate.Date - period.StartDate.Date).Days;
    }

    private static RentalPriceSegmentDto MapSegment(RentalPriceSegment segment)
    {
        return new RentalPriceSegmentDto(
            Days: segment.Days,
            BaseDailyPrice: segment.BaseDailyPrice,
            DurationDiscountPercent: segment.DurationDiscountPercent,
            DiscountedDailyPrice: segment.DiscountedDailyPrice,
            BaseRentalPrice: segment.BaseRentalPrice,
            DiscountAmount: segment.DiscountAmount,
            Total: segment.Total,
            AppliedForDays: segment.AppliedForDays);
    }
}