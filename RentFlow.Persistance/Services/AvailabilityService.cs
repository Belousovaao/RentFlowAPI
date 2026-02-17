using System;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Persistance;

namespace RentFlow.Domain.Bookings.Services;

public class AvailabilityService : IAvailabilityService
{ 
    private readonly RentFlowDbContext _db;
    public AvailabilityService(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsAvailable(Guid assetId, RentalPeriod period, CancellationToken ct = default)
    {
        return !await _db.Bookings.AnyAsync(b => 
        b.AssetId == assetId && b.Status != BookingStatus.Cancelled &&
        b.RentalPeriod.StartDate < period.EndDate && b.RentalPeriod.EndDate > period.StartDate, ct);
    }
}
