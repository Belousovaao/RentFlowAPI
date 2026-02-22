using System;

namespace RentFlow.Domain.Common;

public sealed class BookingConflictException : DomainException
{
    public BookingConflictException() : base("Booking period conflicts with the existing booking.")
    {
    }
}
