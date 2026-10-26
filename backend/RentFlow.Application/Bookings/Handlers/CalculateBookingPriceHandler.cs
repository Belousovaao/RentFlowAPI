using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Common;

namespace RentFlow.Application.Bookings.Handlers;

public class CalculateBookingPriceHandler : IRequestHandler<CalculateBookingPriceQuery, BookingCalculationResultDto>
{
    private readonly IAssetRepository _assets;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookingPricingService _pricingService;
    private readonly IAdditionalServiceRepository _additionalServiceRepository;
    private readonly IBookingDestinationResolver _destinationResolver;

    public CalculateBookingPriceHandler(
        IAssetRepository asset,
        IDateTimeProvider dateTimeProvider,
        IBookingPricingService pricingService,
        IAdditionalServiceRepository additionalServiceRepository,
        IBookingDestinationResolver bookingDestinationResolver)
        {
            _assets = asset; 
            _dateTimeProvider = dateTimeProvider; 
            _pricingService = pricingService;
            _additionalServiceRepository = additionalServiceRepository;
            _destinationResolver = bookingDestinationResolver;
        }
    public async Task<BookingCalculationResultDto> Handle(CalculateBookingPriceQuery cmd, CancellationToken ct)
    {
        var startUtc = _dateTimeProvider.ToUtc(cmd.PickupDate, cmd.PickupTime);
        var endUtc = _dateTimeProvider.ToUtc(cmd.ReturnDate, cmd.ReturnTime);

        RentalPeriod period = new RentalPeriod(startUtc, endUtc);

        var asset = await _assets.GetByIdAsync(cmd.AssetId, ct)
            ?? throw new AssetNotFoundException();

        //----------------------------------------------------
        // Дополнительные услуги
        //----------------------------------------------------

        var services = await _additionalServiceRepository.GetByIdsAsync(
            cmd.Services.Select(x => x.AdditionalServiceId), ct);

        var selectedServices = services
            .Join(
                cmd.Services,
                s => s.Id,
                d => d.AdditionalServiceId,
                (service, dto) => new SelectedAdditionalService(
                    service,
                    dto.SelectedTime))
            .ToList();

        var resolvedDestination = await _destinationResolver.ResolveAsync(
            cmd.DestinationType,
            cmd.DestinationId,
            cmd.CustomRouteDescription,
            ct);

        //----------------------------------------------------
        // Цена
        //----------------------------------------------------

        BookingCalculationResult price = _pricingService.Calculate(
            asset,
            period,
            selectedServices,
            resolvedDestination.RouteSurcharge);

        IReadOnlyList<BookingServiceDto> bookingResultServices = price.Services
                .Select(x => new BookingServiceDto(
                    x.Service.Id,
                    x.Service.Type,
                    x.Service.Name,
                    x.Price,
                    x.SelectedTime))
                .ToList();

        var rentalPriceDto = new RentalPriceDto(
            price.Price.RentalPrice.Segments
                .OrderBy(x => x.AppliedForDays)
                .Select(x => new RentalPriceSegmentDto(
                    x.Days,
                    x.BaseDailyPrice,
                    x.DurationDiscountPercent,
                    x.DiscountedDailyPrice,
                    x.BaseRentalPrice,
                    x.DiscountAmount,
                    x.Total,
                    x.AppliedForDays
                ))
                .ToList(),

            price.Price.RentalPrice.BaseRentalPrice,
            price.Price.RentalPrice.DiscountAmount,
            price.Price.RentalPrice.RentalPriceWithDiscount
        );

        var bookingPriceDto = new BookingPriceDto(
            rentalPriceDto,
            price.Price.Deposit,
            price.Price.ServicesPrice,
            price.Price.RouteSurcharge,
            price.Price.Total,
            price.Price.TotalToPay,
            price.Price.Penalties.Select(x => new BookingPenaltyDto(
                x.Id,
                x.Type,
                x.Amount)).ToList(),
            price.Price.PenaltiesPrice
        );

        return new BookingCalculationResultDto(bookingPriceDto, bookingResultServices);
    }
}
