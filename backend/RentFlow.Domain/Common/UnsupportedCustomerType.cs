using System;

namespace RentFlow.Domain.Common;

public sealed class UnsupportedCustomerType : DomainException
{
    public UnsupportedCustomerType() : base("Invalid customer type")
    {
    }
}
