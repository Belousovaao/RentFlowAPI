using System;

namespace RentFlow.Domain.Bookings;

public class RentalPeriod
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
