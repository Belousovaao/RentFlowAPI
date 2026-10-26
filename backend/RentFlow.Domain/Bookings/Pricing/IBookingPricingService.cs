using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Domain.Bookings.Pricing;

public interface IBookingPricingService
{
    BookingCalculationResult Calculate(
        Asset asset,
        RentalPeriod rentalPeriod,
        IReadOnlyCollection<SelectedAdditionalService> services,
        decimal routeSurcharge = 0);

    RentalPrice CalculateExtensionPrice(
        RentalPeriod originalPeriod,
        RentalPeriod extendedPeriod,
        RentalPrice existingPrice);

    public RentalPrice CalculateReductionPrice(
        RentalPeriod originalPeriod, 
        RentalPeriod reducedPeriod, 
        RentalPrice existingPrice);
    
    public RentalPrice CalculatePeriodChangePrice(
        RentalPeriod originalPeriod,
        RentalPeriod newPeriod,
        RentalPrice existingPrice);

    public BookingServicesCalculationResult CalculateServices(
        Asset asset,
        IReadOnlyCollection<SelectedAdditionalService> services);

    public BookingPenalty CalculateReductionPenalty(RentalPrice existingPrice);
}