using System;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Common;
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
    public BookingRole? Signatory { get; private set; }

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

    private static readonly Dictionary<BookingStatus, BookingStatus[]> TransitionAllowed = new()
    {
        {BookingStatus.Pending, new[] {BookingStatus.Confirmed, BookingStatus.Cancelled}},
        {BookingStatus.Confirmed, new[] {BookingStatus.Active, BookingStatus.Cancelled}},
        {BookingStatus.Active, new[] {BookingStatus.Completed}},
    };

    private void EnsureTransitionAllowed(BookingStatus status)
    {
        if (!TransitionAllowed.TryGetValue(Status, out var allowed) || !allowed.Contains(status))
            throw new InvalidStatusTransitionException();
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

    public void AddRole(BookingRoleType role, Guid personId)
    {
        if (role == BookingRoleType.Signatory && _roles.Any(r => r.RoleType == BookingRoleType.Signatory))
        {
            throw new DublicateSignatoryException();
        }
        _roles.Add(new BookingRole(personId, role));
    }

    public static Booking Create(Asset asset, Customer customer, RentalPeriod period)
    {
        if (asset.Status != AssetStatus.Available)
            throw new BookingConflictException();

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

            _ => throw new UnsupportedCustomerType()
        };

        decimal totalPrice = asset.DailyPrice * (decimal)period.TotalDays;

        return new Booking(asset.Id, customer.Id, period, totalPrice, assetSnapshot, customerSnapshot);
    }

    public void ChangePeriod(RentalPeriod newPeriod, decimal newTotalPrice)
    {
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed)
            throw new BookingModificationNotAllowedException();

        RentalPeriod = newPeriod;
        TotalPrice = newTotalPrice;
    }

    public void SetSignatory(Guid personId)
    {
        if (Signatory != null)
            throw new DublicateSignatoryException();

        Signatory = new BookingRole(personId, BookingRoleType.Signatory);
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
