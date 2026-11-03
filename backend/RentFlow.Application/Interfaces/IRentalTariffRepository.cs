using RentFlow.Domain.Settings;

namespace RentFlow.Application.Interfaces;

public interface IRentalTariffRepository
{
    Task<IReadOnlyList<RentalTariff>> GetAllAsync(CancellationToken ct = default);
    Task<RentalTariff?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task AddAsync(RentalTariff tariff, CancellationToken ct = default);
}
