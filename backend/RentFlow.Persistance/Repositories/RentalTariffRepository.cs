using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Settings;

namespace RentFlow.Persistance.Repositories;

public sealed class RentalTariffRepository : IRentalTariffRepository
{
    private readonly RentFlowDbContext _db;

    public RentalTariffRepository(RentFlowDbContext db) => _db = db;

    public async Task<IReadOnlyList<RentalTariff>> GetAllAsync(CancellationToken ct = default) =>
        await _db.RentalTariffs.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);

    public Task<RentalTariff?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        _db.RentalTariffs.FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task AddAsync(RentalTariff tariff, CancellationToken ct = default) =>
        await _db.RentalTariffs.AddAsync(tariff, ct);
}
