using System;
using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Assets;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Assets.Queries;

namespace RentFlow.Persistance.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly RentFlowDbContext _db;

    public AssetRepository(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task<Asset?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _db.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task<IReadOnlyList<Asset>> SearchAsync(AssetFilter filter, CancellationToken ct = default)
    {
        IQueryable<Asset> query = _db.Assets.AsNoTracking();

        if (filter.Type.HasValue)
            query = query.Where(x => x.Type == filter.Type.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x =>
                x.Code.ToLower().Contains(s) ||
                x.Name.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(x => x.Category == filter.Category);

        if (filter.PriceFrom.HasValue)
            query = query.Where(x => x.DailyPrice >= filter.PriceFrom.Value);

        if (filter.PriceTo.HasValue)
            query = query.Where(x => x.DailyPrice <= filter.PriceTo.Value);

        if (filter.LocationId.HasValue)
            query = query.Where(x => x.LocationId == filter.LocationId.Value);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

}
