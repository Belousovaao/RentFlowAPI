using RentFlow.Domain.Common;

namespace RentFlow.Domain.Bookings.ValueObjects;

public class RentalPeriod
{
    public DateTime StartDate { get; }

    public DateTime EndDate { get; }

    private RentalPeriod()
    {
    }

    public RentalPeriod(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new InvalidRentalPeriodException();

        StartDate = start;
        EndDate = end;
    }

    public bool Intersects(RentalPeriod other)
    {
        return StartDate < other.EndDate &&
               EndDate > other.StartDate;
    }
}