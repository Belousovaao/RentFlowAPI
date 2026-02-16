using System;

namespace RentFlow.Application.Bookings.Dtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public string AssetName { get; set; }
    public string CustomerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}
