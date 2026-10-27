using MediatR;

namespace RentFlow.Application.RentalTariffs;

public sealed record UpsertRentalTariffCommand(string Code, decimal Price, string Unit) : IRequest<RentalTariffDto>;
