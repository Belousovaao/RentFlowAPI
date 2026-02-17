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

    private Booking() {}

    public Booking(Guid assetId, Guid customerId, RentalPeriod period, decimal totalPrice)
    {
        Id = Guid.NewGuid();
        AssetId = assetId;
        CustomerId = customerId;
        RentalPeriod = period;
        TotalPrice = totalPrice;
        Status = BookingStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Может быть подтверждено только бронирование в статусе ожидания");
        
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Может быть завершено только бронирование в статусе подтверждено");
        Status = BookingStatus.Completed;
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
    Completed
}
