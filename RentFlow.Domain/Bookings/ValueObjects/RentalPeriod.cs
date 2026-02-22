using System;

namespace RentFlow.Domain.Bookings;

public class RentalPeriod
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public int TotalDays => (EndDate - StartDate).Days;

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
