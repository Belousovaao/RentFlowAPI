using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Queries;

public sealed class PreviewBookingServicesChangeHandler
    : IRequestHandler<
        PreviewBookingServicesChangeQuery,
        BookingServicesChangePreviewDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAdditionalServiceRepository _additionalServiceRepository;
    private readonly IBookingPricingService _pricingService;

    public PreviewBookingServicesChangeHandler(
        IBookingRepository bookingRepository,
        IAssetRepository assetRepository,
        IAdditionalServiceRepository additionalServiceRepository,
        IBookingPricingService pricingService)
    {
        _bookingRepository = bookingRepository;
        _assetRepository = assetRepository;
        _additionalServiceRepository = additionalServiceRepository;
        _pricingService = pricingService;
    }

    public async Task<BookingServicesChangePreviewDto> Handle(PreviewBookingServicesChangeQuery request, CancellationToken ct)
    {
        var booking = await _bookingRepository.GetByIdAsync(
            request.BookingId,
            ct);

        if (booking is null)
            throw new KeyNotFoundException($"Бронирование {request.BookingId} не найдено.");

        if (request.CustomerId.HasValue && booking.CustomerId != request.CustomerId.Value)
        {
            throw new UnauthorizedAccessException("Нет доступа к этому бронированию.");
        }

        if (booking.Status is not (
            BookingStatus.Pending or
            BookingStatus.Confirmed or
            BookingStatus.Active))
        {
            throw new InvalidOperationException("Дополнительные услуги нельзя изменить в текущем статусе бронирования.");
        }

        var asset = await _assetRepository.GetByIdAsync(
            booking.AssetId,
            ct);

        if (asset is null)
            throw new KeyNotFoundException($"Автомобиль {booking.AssetId} не найден.");

        var serviceIds = request.Services
            .Select(x => x.AdditionalServiceId)
            .Distinct()
            .ToList();

        var services = await _additionalServiceRepository
            .GetByIdsAsync(serviceIds, ct);

        if (services.Count != serviceIds.Count)
        {
            var foundIds = services
                .Select(x => x.Id)
                .ToHashSet();

            var missingIds = serviceIds
                .Where(id => !foundIds.Contains(id))
                .ToList();

            throw new KeyNotFoundException($"Дополнительные услуги не найдены: " + $"{string.Join(", ", missingIds)}");
        }

        var servicesById = services.ToDictionary(x => x.Id);

        var selectedServices = request.Services
            .Select(x =>
            {
                var service = servicesById[x.AdditionalServiceId];

                return new SelectedAdditionalService(
                    service,
                    x.SelectedTime);
            })
            .ToList();

        var calculation = _pricingService.CalculateServices(
            asset,
            selectedServices);

        var newServices = calculation.Services
            .Select(x => new BookingServiceDto(
                x.Service.Id,
                x.Service.Type,
                x.Service.Name,
                x.Price,
                x.SelectedTime))
            .ToList();

        var currentServicesPrice = booking.Price.ServicesPrice;
        var newServicesPrice = calculation.servicePrice;

        var difference = newServicesPrice - currentServicesPrice;

        var currentTotal = booking.Price.Total;

        var newTotal = currentTotal + difference;

        return new BookingServicesChangePreviewDto(
            currentServicesPrice,
            newServicesPrice,
            difference,
            currentTotal,
            newTotal,
            newServices);
    }
}