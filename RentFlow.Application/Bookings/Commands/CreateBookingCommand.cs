using System;

namespace RentFlow.Application.Bookings.Commands;

public record CreateBookingCommand
{
    public Guid AssetId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverPersonId { get; set; }
    public DateTime StartDate { get; set;}
    public DateTime EndDate { get; set; }
}
