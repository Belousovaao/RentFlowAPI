using System;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Handlers;

public class CreateBookingHandler
{
    private readonly IBookingRepository _bookings;
    private readonly IAvailabilityService _availability;
    private readonly IUnitOfWork _uow;
    private readonly IAssetRepository _assets;
    private readonly ICustomerRepository _customers;

    public CreateBookingHandler(
        IUnitOfWork unitOfWork, 
        IBookingRepository bookings, 
        IAvailabilityService availability, 
        IAssetRepository asset,
        ICustomerRepository customer)
    {
        _uow = unitOfWork;
        _bookings = bookings;
        _availability = availability;
        _assets = asset;
        _customers = customer;
    }

    public async Task<Booking> Handle(CreateBookingCommand cmd, CancellationToken ct)
    {
        RentalPeriod period = new RentalPeriod(cmd.StartDate, cmd.EndDate);

        await _uow.BeginTransactionAsync(ct);

        bool available = await _availability.IsAvailable(cmd.AssetId, period, ct);

        if (!available)
        {
            throw new InvalidOperationException("Asset is not available");
        }
        
        Asset asset = await _assets.GetByIdAsync(cmd.AssetId, ct)
        ?? throw new InvalidOperationException("Asset not found");

        Customer customer = await _customers.GetByIdAsync(cmd.CustomerId, ct)
        ?? throw new InvalidOperationException("Customer not found");

        Booking booking = Booking.Create(asset, customer, period);

        booking.AddRole(new BookingRole(cmd.DriverPersonId, BookingRoleType.Driver));

        await _bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitAsync(ct);

        return booking;
    }

}
