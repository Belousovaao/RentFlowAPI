using System;

namespace RentFlow.Domain.Common;

public sealed class BookingModificationNotAllowedException : DomainException
{
    public BookingModificationNotAllowedException() : base("Booking modification is nor allowed")
    {
    }
}
