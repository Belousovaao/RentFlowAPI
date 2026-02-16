using System;

namespace RentFlow.Domain.Bookings;

public class PricingRule
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Multiplier { get; set; } // например 1.0 = базовая цена, 1.2 = +20%
    public string? Condition { get; set; } // описание правила
}
