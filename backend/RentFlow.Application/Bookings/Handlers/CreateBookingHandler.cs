using MediatR;
using Microsoft.Extensions.Logging;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Pricing;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Common;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Handlers;

public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Booking>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;
    private readonly IAssetRepository _assets;
    private readonly ICustomerRepository _customers;
    private readonly ILogger<CreateBookingHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookingPricingService _pricingService;
    private readonly IAdditionalServiceRepository _additionalServiceRepository;
    private readonly IBookingDestinationResolver _destinationResolver;

    public CreateBookingHandler(
        IUnitOfWork unitOfWork, 
        IBookingRepository bookings, 
        IAssetRepository asset,
        ICustomerRepository customer, 
        IDateTimeProvider dateTimeProvider,
        IBookingPricingService pricingService,
        IAdditionalServiceRepository additionalServiceRepository,
        ILogger<CreateBookingHandler> logger,
        IBookingDestinationResolver destinationResolver)
    {
        _uow = unitOfWork;
        _bookings = bookings;
        _assets = asset;
        _customers = customer;
        _dateTimeProvider = dateTimeProvider;
        _pricingService = pricingService;
        _additionalServiceRepository = additionalServiceRepository;
        _logger = logger;
        _destinationResolver = destinationResolver;
    }

    public async Task<Booking> Handle(CreateBookingCommand cmd, CancellationToken ct)
    {
        var pickupTime = cmd.PickupTime ?? new TimeOnly(12, 0);
        var returnTime = cmd.ReturnTime ?? new TimeOnly(12, 0);
        var startUtc = _dateTimeProvider.ToUtc(cmd.PickupDate, pickupTime);
        var endUtc = _dateTimeProvider.ToUtc(cmd.ReturnDate, returnTime);
        RentalPeriod period = new RentalPeriod(startUtc, endUtc);
        
        Asset asset = await _assets.GetByIdAsync(cmd.AssetId, ct) ?? throw new AssetNotFoundException();

        Customer customer = await _customers.GetByIdAsync(cmd.CustomerId, ct) ?? throw new CustomerNotFoundException();

        var resolvedDestination = await _destinationResolver.ResolveAsync(
            cmd.DestinationType,
            cmd.DestinationId,
            cmd.CustomRouteDescription,
            ct);

        var selectedServices = new List<SelectedAdditionalService>();
        var services = await _additionalServiceRepository.GetByIdsAsync(cmd.Services.Select(x => x.AdditionalServiceId), ct);
        var serviceDict = services.ToDictionary(x => x.Id);

        foreach (var dto in cmd.Services)
        {
            if (!serviceDict.TryGetValue(dto.AdditionalServiceId, out var service))
                throw new BusinessRuleViolationException("Additional service not found.");

            selectedServices.Add(
                new SelectedAdditionalService(
                    service,
                    dto.SelectedTime));
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            List<Booking> overlapping = await _bookings.GetOverlappingAsync(cmd.AssetId, period, ct);

            if (overlapping.Any())
                throw new BookingConflictException();

            Driver? driver = null;
            if (cmd.Driver is not null)
            {
                driver = Driver.Create(
                    new PersonName(cmd.Driver.FirstName, cmd.Driver.LastName, cmd.Driver.MiddleName),
                    new DriverLicense(cmd.Driver.LisenceNumber, cmd.Driver.Categories, cmd.Driver.IssuedDate, cmd.Driver.ExpirationDate, cmd.Driver.ExprerienceFromYear),
                    cmd.Driver.Phone
                );
            }
            if (cmd.Driver is null && customer is Individual)
                driver = Driver.FromIndividual((Individual)customer);

            if (cmd.Driver is null && customer is IndividualEntrepreneur)
                driver = Driver.FromEntrepreneur((IndividualEntrepreneur)customer);
            
            if (cmd.Driver is null && customer is Organization)
                throw new BusinessRuleViolationException("Organization must have a driver");

            var price = _pricingService.Calculate(asset, period, selectedServices, resolvedDestination.RouteSurcharge);    

            Booking booking = Booking.Create(asset, customer, driver, period, price.Price, resolvedDestination.Destination);

            foreach(var service in price.Services)
            {
                booking.AddService(
                    new BookingService(
                        service.Service,
                        service.Service.Type,
                        service.Price,
                        service.SelectedTime));
            }

            await _bookings.AddAsync(booking, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation(BookingLogEvent.Created, $"Booking created: {booking.Id} {booking.AssetId} {booking.CustomerId} {booking.RentalPeriod}");

            return booking;
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

}
