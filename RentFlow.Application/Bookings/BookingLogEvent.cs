using System;
using Microsoft.Extensions.Logging;

namespace RentFlow.Application.Bookings;

public static class BookingLogEvent
{
    public static readonly EventId Created = new(2001, nameof(Created));
    public static readonly EventId Updated = new(2002, nameof(Updated));
    public static readonly EventId Cancelled = new(2003, nameof(Cancelled));
    public static readonly EventId Confirmed = new(2004, nameof(Confirmed));
}
