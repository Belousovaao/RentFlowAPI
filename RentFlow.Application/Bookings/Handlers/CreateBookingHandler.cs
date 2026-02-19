using System;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings;
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

        BookingAssetSnapshot assetSnapShot = new BookingAssetSnapshot(
            asset.Name,
            asset.Type,
            asset.Category,
            asset.DailyPrice,
            asset.Deposit,
            asset.CanDeliver,
            asset.DeliveryPrice);

        BookingIndividualSnapshot? individualSnapshot = null;
        BookingEntrepreneurSnapshot? entrepreneurSnapshot = null;
        BookingOrganizationSnapshot? organizationSnapshot = null;

        switch (customer)
        {
            case Individual individual:
                individualSnapshot = new BookingIndividualSnapshot(
                    individual.Name,
                    individual.IndividualPassport,
                    individual.Email,
                    individual.Phone);
                break;

            case IndividualEntrepreneur entrepreneur:
                entrepreneurSnapshot = new BookingEntrepreneurSnapshot(
                    entrepreneur.Name,
                    entrepreneur.OrganizationAdress,
                    entrepreneur.FactAdress,
                    entrepreneur.IPBankAccount,
                    entrepreneur.Phone,
                    entrepreneur.Email);
                break;

            case Organization organization:
                organizationSnapshot = new BookingOrganizationSnapshot(
                    organization.FullName,
                    organization.ShortName,
                    organization.KPP,
                    organization.OrganizationAdress,
                    organization.FactAdress,
                    organization.OrganizationBankAccount);
                break;

            default:
                throw new InvalidOperationException("Unsupported customer type");
        }
        
        decimal totalPrice = assetSnapShot.DailyPrice * (decimal) (cmd.EndDate -cmd.StartDate).TotalDays;
        Booking booking = new Booking(
            asset.Id, 
            cmd.CustomerId, 
            period, 
            totalPrice, 
            assetSnapShot,
            individualSnapshot,
            entrepreneurSnapshot,
            organizationSnapshot);
        booking.AddRole(new BookingRole(cmd.DriverPersonId, BookingRoleType.Driver));

        await _bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitAsync(ct);

        await _bookings.AddAsync(booking);

        return booking;

    }

}
