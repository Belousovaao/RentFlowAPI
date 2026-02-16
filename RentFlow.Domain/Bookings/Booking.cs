using System;
using RentFlow.Domain.Assets;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class Booking
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public Guid RentalPeriodId { get; set; }
    public RentalPeriod RentalPeriod { get; set; }

    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}
