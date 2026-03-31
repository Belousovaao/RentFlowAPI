using System;

namespace RentFlow.Domain.Common;

public sealed class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException() : base("This status is impossible to set")
    {
    }
}
