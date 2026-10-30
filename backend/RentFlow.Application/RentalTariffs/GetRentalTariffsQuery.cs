using MediatR;

namespace RentFlow.Application.RentalTariffs;

public sealed record GetRentalTariffsQuery : IRequest<IReadOnlyList<RentalTariffDto>>;
