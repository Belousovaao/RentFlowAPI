using System;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Interfaces;

public interface IAvailabilityService
{
    Task<bool> IsAvailable(Guid assetId, RentalPeriod period, CancellationToken ct = default);
}
