using MediatR;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.RentalTariffs;

public sealed class GetRentalTariffsHandler : IRequestHandler<GetRentalTariffsQuery, IReadOnlyList<RentalTariffDto>>
{
    private readonly IRentalTariffRepository _tariffs;

    public GetRentalTariffsHandler(IRentalTariffRepository tariffs) => _tariffs = tariffs;

    public async Task<IReadOnlyList<RentalTariffDto>> Handle(GetRentalTariffsQuery request, CancellationToken ct)
    {
        var tariffs = await _tariffs.GetAllAsync(ct);
        return tariffs.Select(x => new RentalTariffDto(x.Code, x.Price, x.Unit, x.UpdatedAtUtc)).ToList();
    }
}
