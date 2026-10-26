using System;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Bookings.ValueObjects;
using RentFlow.Domain.Common;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class Booking
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid CustomerId { get; private set; }
    public RentalPeriod RentalPeriod { get; private set; } = null!;
    public BookingPrice Price { get; private set; } = null!;
    public BookingStatus Status { get; private set; }
    public BookingDestination Destination { get; private set; } = null!;
    private readonly List<BookingRole> _roles = new List<BookingRole>();
    public IReadOnlyCollection<BookingRole> Roles => _roles;
    public BookingAssetSnapshot AssetSnapshot { get; private set; }
    public BookingCustomerSnapshot CustomerSnapshot { get; private set; }
    public BookingRole? Signatory { get; private set; }
     public Guid? SignatoryId { get; private set; }
    public Driver BookingDriver { get; private set; }

    private readonly List<BookingService> _services = new();

    public IReadOnlyCollection<BookingService>Services => _services;
    private Booking() {}

    public Booking(
        Guid assetId, 
        Guid customerId, 
        Driver driver,
        RentalPeriod period, 
        BookingPrice price, 
        BookingAssetSnapshot assetSnapshot, 
        BookingCustomerSnapshot customerSnapshot,
        BookingDestination bookingDestination)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        CustomerId = customerId;
        BookingDriver = driver;
        RentalPeriod = period;
        Price = price;
        AssetSnapshot = assetSnapshot;
        CustomerSnapshot = customerSnapshot;
        Status = BookingStatus.Pending;
        Destination = bookingDestination;
    }

    private static readonly Dictionary<BookingStatus, BookingStatus[]> TransitionAllowed = new()
    {
        {BookingStatus.Pending, new[] {BookingStatus.Confirmed, BookingStatus.Cancelled}},
        {BookingStatus.Confirmed, new[] {BookingStatus.Active, BookingStatus.Cancelled}},
        {BookingStatus.Active, new[] {BookingStatus.Completed}},
    };

    private void EnsureTransitionAllowed(
        BookingStatus status,
        Customer? customer)
    {
        if (!TransitionAllowed.TryGetValue(Status, out var allowed) ||
            !allowed.Contains(status))
        {
            throw new InvalidStatusTransitionException();
        }

        if (status is BookingStatus.Confirmed or BookingStatus.Active)
        {
            if (customer is null ||
                customer.VerificationStatus != VerificationStatus.Approved)
            {
                throw new InvalidStatusTransitionException();
            }
        }
    }
    public void Confirm(Customer customer)
    {
        EnsureTransitionAllowed(BookingStatus.Confirmed, customer);
        
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        EnsureTransitionAllowed(BookingStatus.Cancelled, null);
        Status = BookingStatus.Cancelled;
    }

    public void Complete()
    {
        EnsureTransitionAllowed(BookingStatus.Completed, null);
        Status = BookingStatus.Completed;
    }

    public void Activate(Customer customer)
    {
        EnsureTransitionAllowed(BookingStatus.Active, customer);
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

    public static Booking Create(Asset asset, Customer customer, Driver? driver, RentalPeriod period, BookingPrice price, BookingDestination destination)
    {
        if (asset.Status != AssetStatus.Available)
            throw new BookingConflictException();

        Driver bookingDriver = customer.ResolveDriver(driver);

        BookingAssetSnapshot assetSnapshot = new BookingAssetSnapshot(
            asset.BrandName,
            asset.Model,
            asset.Year,
            asset.Type,
            asset.Category);
        
        BookingCustomerSnapshot customerSnapshot = BookingCustomerSnapshot.FromCustomer(customer);
    
        return new Booking(
            asset.Id, 
            customer.Id, 
            bookingDriver, 
            period, 
            price, 
            assetSnapshot, 
            customerSnapshot,
            destination);
    }

    public void ChangePeriod(RentalPeriod newPeriod, RentalPrice newPrice, BookingPenalty? penalty = null)
    {
        EnsurePeriodModificationAllowed();

        var currentDays = GetRentalDays(RentalPeriod);
        var newDays = GetRentalDays(newPeriod);

        if (newDays > currentDays)
        {
            ExtendPeriod(newPeriod, newPrice);
        }
        else if (newDays < currentDays)
        {
            ReducePeriod(newPeriod, newPrice);
            if (penalty is not null)
            {
                Price.AddPenalty(penalty);
            }
        }
        else
        {
            throw new ArgumentException(
                "Новый срок аренды не изменился.");
        }
    }

    public void ExtendPeriod(RentalPeriod newPeriod, RentalPrice newPrice) 
    { 
        EnsurePeriodModificationAllowed(); 
        var currentDays = GetRentalDays(RentalPeriod); 
        var newDays = GetRentalDays(newPeriod); 
        if (newDays <= currentDays) 
        {
            throw new ArgumentException( "Новый срок аренды при продлении должен быть больше текущего.");
        }
        RentalPeriod = newPeriod; 
        Price.UpdateRentalPrice(newPrice); 
    }

    public void ReducePeriod(RentalPeriod newPeriod, RentalPrice newPrice) 
    { 
        EnsurePeriodModificationAllowed(); 
        var currentDays = GetRentalDays(RentalPeriod); 
        var newDays = GetRentalDays(newPeriod); 
        if (newDays >= currentDays) 
        {
            throw new ArgumentException( "Новый срок аренды при сокращении должен быть меньше текущего.");
        }
        RentalPeriod = newPeriod; 
        Price.UpdateRentalPrice(newPrice); 
    }

    public void SetSignatory(Guid personId)
    {
        if (Signatory != null)
            throw new DublicateSignatoryException();

        Signatory = new BookingRole(personId, BookingRoleType.Signatory);
    }

    public void AddService(BookingService service)
    {
        _services.Add(service);
    }

    public void RemoveService(Guid serviceId)
    {
        var service = _services.First(x => x.Id == serviceId);
        _services.Remove(service);
    }

    private void EnsurePeriodModificationAllowed() 
    { 
        if (Status is BookingStatus.Cancelled or BookingStatus.Completed) 
            throw new BookingModificationNotAllowedException(); 
    } 
    private static int GetRentalDays(RentalPeriod period) 
    { 
        return (period.EndDate.Date - period.StartDate.Date).Days; 
    }

    public bool IsPeriodReduction(RentalPeriod newPeriod)
    {
        return GetRentalDays(newPeriod) < GetRentalDays(RentalPeriod);
    }

    public bool HasPeriodReductionPenalty => Price.HasPenalty(BookingPenaltyType.PeriodReduction);
}


public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed,
    Active
}
