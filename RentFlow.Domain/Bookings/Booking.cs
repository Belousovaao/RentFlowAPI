using System;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid CustomerId { get; private set; }
    public RentalPeriod RentalPeriod { get; private set; }
    public decimal TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    private readonly List<BookingRole> _roles = new List<BookingRole>();
    public IReadOnlyCollection<BookingRole> Roles => _roles;
    public BookingAssetSnapshot AssetSnapshot { get; private set; }
    public BookingCustomerSnapshot CustomerSnapshot { get; private set; }

    private Booking() {}

    public Booking(
        Guid assetId, 
        Guid customerId, 
        RentalPeriod period, 
        decimal totalPrice, 
        BookingAssetSnapshot assetSnapshot, 
        BookingCustomerSnapshot customerSnapshot)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        CustomerId = customerId;
        RentalPeriod = period;
        TotalPrice = totalPrice;
        AssetSnapshot = assetSnapshot;
        CustomerSnapshot = customerSnapshot;
        Status = BookingStatus.Pending;
    }

    private void EnsureTransitionAllowed(BookingStatus target)
    {
        bool allowed = Status switch
        {
            BookingStatus.Pending =>
                target is BookingStatus.Confirmed or BookingStatus.Cancelled,

            BookingStatus.Confirmed =>
                target is BookingStatus.Active or BookingStatus.Cancelled,

            BookingStatus.Active => target is BookingStatus.Completed,

            BookingStatus.Completed => false,

            BookingStatus.Cancelled => false,

            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException($"Transition from {Status} to {target} is not allowed.");   
    }


    public void Confirm()
    {
        EnsureTransitionAllowed(BookingStatus.Confirmed);
        
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        EnsureTransitionAllowed(BookingStatus.Cancelled);
        Status = BookingStatus.Cancelled;
    }

    public void Complete()
    {
        EnsureTransitionAllowed(BookingStatus.Completed);
        Status = BookingStatus.Completed;
    }

    public void Activate()
    {
        EnsureTransitionAllowed(BookingStatus.Active);
        Status = BookingStatus.Active;
    }

    public void AddRole(BookingRole role)
    {
        _roles.Add(role);
    }

    public static Booking Create(Asset asset, Customer customer, RentalPeriod period)
    {
        BookingAssetSnapshot assetSnapshot = new BookingAssetSnapshot(
            asset.Name,
            asset.Type,
            asset.Category,
            asset.DailyPrice,
            asset.Deposit,
            asset.CanDeliver,
            asset.DeliveryPrice);
        
        BookingCustomerSnapshot customerSnapshot = customer switch
        {
            Individual individual => new BookingIndividualSnapshot(
                individual.Name,
                individual.IndividualPassport,
                individual.Email,
                individual.Phone),

            IndividualEntrepreneur entrepreneur => new BookingEntrepreneurSnapshot(
                entrepreneur.Name,
                entrepreneur.OrganizationAdress,
                entrepreneur.FactAdress,
                entrepreneur.IPBankAccount,
                entrepreneur.Phone,
                entrepreneur.Email),

            Organization organization => new BookingOrganizationSnapshot(
                organization.FullName,
                organization.ShortName,
                organization.KPP,
                organization.OrganizationAdress,
                organization.FactAdress,
                organization.OrganizationBankAccount),

            _ => throw new InvalidOperationException("Invalid customer type")
        };

        decimal totalPrice = asset.DailyPrice * (decimal) (period.EndDate - period.StartDate).TotalDays;

        return new Booking(asset.Id, customer.Id, period, totalPrice, assetSnapshot, customerSnapshot);
    }
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed,
    Active
}
