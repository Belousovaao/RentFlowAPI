using System;
using System.Data;
using Microsoft.Extensions.Logging;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Common;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Handlers;

public class CreateBookingHandler
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;
    private readonly IAssetRepository _assets;
    private readonly ICustomerRepository _customers;
    private readonly ILogger<CreateBookingHandler> _logger;

    public CreateBookingHandler(
        IUnitOfWork unitOfWork, 
        IBookingRepository bookings, 
        IAssetRepository asset,
        ICustomerRepository customer, 
        ILogger<CreateBookingHandler> logger)
    {
        _uow = unitOfWork;
        _bookings = bookings;
        _assets = asset;
        _customers = customer;
        _logger = logger;
    }

    public async Task<Booking> Handle(CreateBookingCommand cmd, CancellationToken ct)
    {
        RentalPeriod period = new RentalPeriod(cmd.StartDate, cmd.EndDate);
        
        Asset asset = await _assets.GetByIdAsync(cmd.AssetId, ct)
        ?? throw new AssetNotFoundException();

        Customer customer = await _customers.GetByIdAsync(cmd.CustomerId, ct)
        ?? throw new CustomerNotFoundException();

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
                    new DriverLicense(cmd.Driver.LisenceNumber, cmd.Driver.Categories, cmd.Driver.IssuedDate, cmd.Driver.ExpirationDate),
                    cmd.Driver.Phone
                );
            }
            if (cmd.Driver is null && customer is Individual)
                driver = Driver.FromIndividual((Individual)customer);

            if (cmd.Driver is null && customer is IndividualEntrepreneur)
                driver = Driver.FromEntrepreneur((IndividualEntrepreneur)customer);
            
            if (cmd.Driver is null && customer is Organization)
                throw new BusinessRuleViolationException("Organization must have a driver");

            Booking booking = Booking.Create(asset, customer, driver, period);

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
