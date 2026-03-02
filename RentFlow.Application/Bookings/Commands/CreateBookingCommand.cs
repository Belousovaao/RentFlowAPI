using System;
using System.Globalization;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Commands;
public sealed record CreateBookingCommand(
    Guid AssetId,
    Guid CustomerId,
    DateTime StartDate,
    DateTime EndDate,
    DriverDto? Driver
);