using System;

namespace RentFlow.Domain.Common;

public sealed class DublicateSignatoryException : DomainException
{
    public DublicateSignatoryException() : base("The signatory can be the only one person")
    {
    }
}
