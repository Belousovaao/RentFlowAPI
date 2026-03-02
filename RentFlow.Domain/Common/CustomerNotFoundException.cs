using System;

namespace RentFlow.Domain.Common;

public sealed class CustomerNotFoundException : DomainException
{

    public CustomerNotFoundException() : base("Customer not found")
    {
        
    }
}
