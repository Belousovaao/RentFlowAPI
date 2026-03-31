using System;

namespace RentFlow.Domain.Common;

public class InvalidRentalPeriodException : DomainException
{
    public InvalidRentalPeriodException() : base("This period is not valid for booking! Try another one")
    {
    }
}
