using System;

namespace RentFlow.Domain.Bookings;

public class RentalPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    private RentalPeriod() {}
    public RentalPeriod(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Дата окончания должна быть позже дата начала");
        StartDate = start;
        EndDate = end;
    }

    public bool Intersects(RentalPeriod other)
    {
        return StartDate < other.EndDate && EndDate > other.StartDate;
    }
}
