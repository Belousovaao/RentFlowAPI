using System;

namespace RentFlow.Domain.Customers;

public class Driver
{
    public Guid Id { get; set; }
    public Guid IndividualId { get; set; }
    public Individual Individual { get; set; }
}
