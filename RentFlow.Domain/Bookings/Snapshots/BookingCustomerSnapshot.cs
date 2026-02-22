using System;

namespace RentFlow.Domain.Bookings.Snapshots;

public abstract class BookingCustomerSnapshot
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
}

