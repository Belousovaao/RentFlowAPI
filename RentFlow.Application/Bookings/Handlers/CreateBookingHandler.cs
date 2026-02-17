using System;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Handlers;

public class CreateBookingHandler
{
    private readonly IBookingRepository _bookings;
    private readonly IAvailabilityService _availability;
    private readonly IUnitOfWork _uow;
    private readonly IAssetRepository _assets;

    public CreateBookingHandler(IUnitOfWork unitOfWork, IBookingRepository bookings, IAvailabilityService availability, IAssetRepository asset)
    {
        _uow = unitOfWork;
        _bookings = bookings;
        _availability = availability;
        _assets = asset;
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
        
        Asset asset = await _assets.GetByIdAsync(cmd.AssetId, ct);
        if (asset == null)
            throw new InvalidOperationException("Asset not found");

        BookingAssetSnapshot assetSnapShot = new BookingAssetSnapshot(
            asset.Name,
            asset.Type,
            asset.Category,
            asset.DailyPrice,
            asset.Deposit,
            asset.CanDeliver,
            asset.DeliveryPrice);

        decimal totalPrice = assetSnapShot.DailyPrice * (decimal) (cmd.EndDate -cmd.StartDate).TotalDays;
        Booking booking = new Booking(asset.Id, cmd.CustomerId, period, totalPrice, assetSnapShot);
        booking.AddRole(new BookingRole(cmd.DriverPersonId, BookingRoleType.Driver));

        await _bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitAsync(ct);

        await _bookings.AddAsync(booking);

        return booking;

    }

}
