using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Commands;

public sealed class UpdateBookingServicesHandler : IRequestHandler<UpdateBookingServicesCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAdditionalServiceRepository _additionalServiceRepository;
    private readonly IBookingPricingService _pricingService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookingServicesHandler(
        IBookingRepository bookingRepository,
        IAssetRepository assetRepository,
        IAdditionalServiceRepository additionalServiceRepository,
        IBookingPricingService pricingService,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _assetRepository = assetRepository;
        _additionalServiceRepository = additionalServiceRepository;
        _pricingService = pricingService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateBookingServicesCommand request, CancellationToken ct)
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
            .Select(requestService =>
            {
                var service = servicesById[requestService.AdditionalServiceId];

                return new SelectedAdditionalService(
                    service,
                    requestService.SelectedTime);
            })
            .ToList();

        var calculation = _pricingService.CalculateServices(
            asset,
            selectedServices);

        var bookingServices = calculation.Services
            .Select(x => new BookingService(
                x.Service,
                x.Service.Type,
                x.Price,
                x.SelectedTime))
            .ToList();

        _bookingRepository.ReplaceBookingServices(booking, bookingServices);

        booking.Price.UpdateServicesPrice(calculation.servicePrice);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}