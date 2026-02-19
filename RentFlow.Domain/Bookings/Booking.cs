using System;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class Booking
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public Guid CustomerId { get; set; }
    public RentalPeriod RentalPeriod { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; private set; }
    private readonly List<BookingRole> _roles = new List<BookingRole>();
    public IReadOnlyCollection<BookingRole> Roles => _roles;
    public BookingAssetSnapshot AssetSnapshot { get; set; }
    public BookingIndividualSnapshot? IndividualSnapshot { get; set; }
    public BookingEntrepreneurSnapshot? EntrepreneurSnapshot { get; set; }
    public BookingOrganizationSnapshot? OrganizationSnapshot { get; set; }

    private Booking() {}

    public Booking(
        Guid assetId, 
        Guid customerId, 
        RentalPeriod period, 
        decimal totalPrice, 
        BookingAssetSnapshot assetSnapshot, 
        BookingIndividualSnapshot? individualSnapshot,
        BookingEntrepreneurSnapshot? entrepreneurSnapshot,
        BookingOrganizationSnapshot? organizationSnapshot)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        CustomerId = customerId;
        RentalPeriod = period;
        TotalPrice = totalPrice;
        AssetSnapshot = assetSnapshot;
        IndividualSnapshot = individualSnapshot;
        EntrepreneurSnapshot = entrepreneurSnapshot;
        OrganizationSnapshot = organizationSnapshot;
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
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed,
    Active
}
