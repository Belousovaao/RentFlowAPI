using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings.Services;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Domain.Bookings.Pricing;

public class BookingPricingService : IBookingPricingService
{
    public BookingCalculationResult Calculate(
        Asset asset,
        RentalPeriod rentalPeriod,
        IReadOnlyCollection<SelectedAdditionalService> services,
        decimal routeSurcharge = 0)
    {
        if (routeSurcharge < 0)
            throw new ArgumentOutOfRangeException(nameof(routeSurcharge));
        RentalPrice rentalPrice = CalculateRentalPrice(asset, rentalPeriod);

        var servicesCalculationResult = CalculateServices(asset, services);

        BookingPrice bookingPrice = new BookingPrice(
            rentalPrice,
            asset.Deposit ?? 0,
            servicesCalculationResult.servicePrice,
            routeSurcharge);
        
        return new BookingCalculationResult(
            bookingPrice,
            servicesCalculationResult.Services);
    }
    private static decimal RoundToHundreds(decimal price)
    {
        var remainder = price % 100;

        if (remainder <= 50)
        {
            return price - remainder;
        }

        return price + (100 - remainder);
    }

    private static decimal GetDiscountPercent(int rentalDays)
    {
        return rentalDays switch
        {
            >= 2 and <= 3 => 0m,
            >= 4 and <= 6 => 5m,
            >= 7 and <= 30 => 10m,
            >= 31 => 25m,
            _ => throw new ArgumentException(
                "Минимальный срок аренды — 2 суток.")
        };
    }
    private RentalPrice CalculateRentalPrice(Asset asset, RentalPeriod period) 
    { 
        var rentalDays = GetRentalDays(period); 
        if (rentalDays < 2)
            throw new ArgumentException("Минимальный срок аренды — 2 суток.");

        var segment = CreateRentalPriceSegment(
            asset.DailyPrice,
            days: rentalDays,
            appliedForDays: rentalDays);

        return new RentalPrice(segment);
    }

    private static int GetRentalDays(RentalPeriod period)
    {
        var days = (period.EndDate.Date - period.StartDate.Date).Days;

        return days;
    }

    private static RentalPriceSegment CreateRentalPriceSegment(decimal baseDailyPrice, int days, int appliedForDays)
    {
        var discountPercent = GetDiscountPercent(appliedForDays);
        var discountedDailyPrice = RoundToHundreds(baseDailyPrice * (1 - discountPercent / 100m));
        var baseRentalPrice = baseDailyPrice * days;
        var total = discountedDailyPrice * days;
        var discountAmount = baseRentalPrice - total;

        return new RentalPriceSegment(
            days: days,
            baseDailyPrice: baseDailyPrice,
            durationDiscountPercent: discountPercent,
            discountedDailyPrice: discountedDailyPrice,
            baseRentalPrice: baseRentalPrice,
            discountAmount: discountAmount,
            total: total,
            appliedForDays: appliedForDays,
            order: 0);
    }

    private decimal CalculateServicePrice(Asset asset, SelectedAdditionalService service)
    {
        return service.Service.Type switch
        {
            ServiceType.Regular =>
                Math.Round(service.Service.Price, MidpointRounding.AwayFromZero),

            ServiceType.EarlyPickup =>
                CalculateEarlyPickupPrice(asset, service.SelectedTime),

            ServiceType.LateReturn =>
                CalculateLateReturnPrice(asset, service.SelectedTime),

            _ => throw new InvalidOperationException()
        };
    }

    private decimal CalculateEarlyPickupPrice(Asset asset, TimeOnly? selectedTime)
    {
        if (selectedTime is null)
            return 0;

        var hours = CalculateChargeableHours(
            new TimeOnly(12, 0),
            selectedTime.Value,
            isEarly: true);

        return Math.Round(hours * (asset.DailyPrice / 24m), MidpointRounding.AwayFromZero);
    }

    private decimal CalculateLateReturnPrice(Asset asset, TimeOnly? selectedTime)
    {
        if (selectedTime is null)
            return 0;

        var hours = CalculateChargeableHours(
            new TimeOnly(12, 0),
            selectedTime.Value,
            isEarly: false);

        return Math.Round(hours * (asset.DailyPrice / 24m), MidpointRounding.AwayFromZero);
    }

    private static int CalculateChargeableHours(TimeOnly defaultTime, TimeOnly selectedTime, bool isEarly) 
    { 
        var minutes = isEarly ? defaultTime.ToTimeSpan() - selectedTime.ToTimeSpan() 
            : selectedTime.ToTimeSpan() - defaultTime.ToTimeSpan(); 
        
        if (minutes <= TimeSpan.Zero) 
            return 0; 
            
        var totalMinutes = minutes.TotalMinutes; 

        // округление до часа 
        var hours = (int)(totalMinutes / 60); 
        
        if (totalMinutes % 60 > 30) 
            hours++; 

        // свыше 3 часов считаем как сутки 
        if (hours > 6) 
            return 24; 
        return hours; 
    }

    public RentalPrice CalculateExtensionPrice(
        RentalPeriod originalPeriod,
        RentalPeriod extendedPeriod,
        RentalPrice existingPrice)
    {
        ArgumentNullException.ThrowIfNull(originalPeriod); 
        ArgumentNullException.ThrowIfNull(extendedPeriod); 
        ArgumentNullException.ThrowIfNull(existingPrice);

        var originalDays = GetRentalDays(originalPeriod);
        var newDays = GetRentalDays(extendedPeriod);

        if (newDays <= originalDays)
            throw new ArgumentException("Новый срок аренды должен быть больше текущего.");

        var extensionDays = newDays - originalDays;
        var lastSegment = existingPrice.Segments.LastOrDefault();

        if (lastSegment is null) 
        { 
            throw new InvalidOperationException( "Стоимость аренды не содержит ни одного ценового сегмента."); 
        }

        var extensionSegment = CreateRentalPriceSegment(
            existingPrice.LastSegment.BaseDailyPrice,
            extensionDays,
            newDays);

        var result = existingPrice.Copy();

        result.AddSegment(extensionSegment);

        return result;
    }

    public RentalPrice CalculateReductionPrice(RentalPeriod originalPeriod, RentalPeriod reducedPeriod, RentalPrice existingPrice)
    {
        ArgumentNullException.ThrowIfNull(originalPeriod);
        ArgumentNullException.ThrowIfNull(reducedPeriod);
        ArgumentNullException.ThrowIfNull(existingPrice);

        var originalDays = GetRentalDays(originalPeriod);
        var newDays = GetRentalDays(reducedPeriod);

        if (newDays <= 0)
            throw new ArgumentException("Новый срок аренды должен содержать хотя бы одну оплачиваемую сутки.");
        var existingSegments = existingPrice.Segments.ToList();

        if (existingSegments.Count == 0)
            throw new InvalidOperationException("Стоимость аренды не содержит ни одного ценового сегмента.");

        var existingPricedDays = existingSegments.Sum(x => x.Days);

        if (newDays >= existingPricedDays)
            throw new ArgumentException("Новый срок аренды должен быть меньше текущего оплачиваемого срока.");

        var resultSegments = new List<RentalPriceSegment>();

        var remainingDays = newDays;

        foreach (var segment in existingSegments)
        {
            if (remainingDays <= 0)
                break;

            // Сегмент полностью остаётся в бронировании.
            if (remainingDays >= segment.Days)
            {
                resultSegments.Add(segment);
                remainingDays -= segment.Days;

                continue;
            }

            // Сокращение попало внутрь этого сегмента.
            // Пересчитываем только оставшуюся часть сегмента.
            var reducedSegment = CreateRentalPriceSegment(
                baseDailyPrice: segment.BaseDailyPrice,
                days: remainingDays,
                appliedForDays: newDays);

            reducedSegment.SetOrder(segment.Order);
            resultSegments.Add(reducedSegment);

            remainingDays = 0;
            break;
        }

        if (remainingDays > 0)
            throw new InvalidOperationException("Количество суток в ценовых сегментах не соответствует текущему периоду аренды.");

        var result = RentalPrice.FromSegments(resultSegments);

        Console.WriteLine("=== CalculateReductionPrice RESULT ===");

        foreach (var segment in result.Segments.OrderBy(x => x.Order))
        {
            Console.WriteLine(
                $"Order={segment.Order}, " +
                $"Id={segment.Id}, " +
                $"Days={segment.Days}, " +
                $"BaseDailyPrice={segment.BaseDailyPrice}, " +
                $"AppliedForDays={segment.AppliedForDays}, " +
                $"Total={segment.Total}");
        }

        return result;
    }

    public RentalPrice CalculatePeriodChangePrice(
        RentalPeriod originalPeriod,
        RentalPeriod newPeriod,
        RentalPrice existingPrice)
    {
        ArgumentNullException.ThrowIfNull(originalPeriod); 
        ArgumentNullException.ThrowIfNull(newPeriod); 
        ArgumentNullException.ThrowIfNull(existingPrice);

        var originalDays = GetRentalDays(originalPeriod);
        var newDays = GetRentalDays(newPeriod);

        if (newDays > originalDays)
        {
            return CalculateExtensionPrice(
                originalPeriod,
                newPeriod,
                existingPrice);
        }

        if (newDays < originalDays)
        {
            return CalculateReductionPrice(
                originalPeriod,
                newPeriod,
                existingPrice);
        }

        throw new ArgumentException("Новый срок аренды не отличается от текущего.");
    }

    public BookingServicesCalculationResult CalculateServices(
        Asset asset,
        IReadOnlyCollection<SelectedAdditionalService> services)
    {
        var calculatedServices = services
            .Select(x =>
                new CalculatedService(
                    x.Service,
                    CalculateServicePrice(asset, x),
                    x.SelectedTime))
            .ToList();

        decimal servicesPrice = calculatedServices.Sum(x => x.Price);

        return new BookingServicesCalculationResult(
            servicesPrice,
            calculatedServices);
    }

    public BookingPenalty CalculateReductionPenalty(RentalPrice existingPrice)
    {
        ArgumentNullException.ThrowIfNull(existingPrice);
        var lastSegment = existingPrice.LastSegment;
        return BookingPenalty.PeriodReduction(lastSegment.DiscountedDailyPrice);
    }

}