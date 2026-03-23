using System;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record BookingDto(
    Guid Id,
    Guid AssetId,
    Guid CustomerId,
    RentalPeriod RentalPeriod,
    decimal TotalPrice,
    BookingStatus Status
);
