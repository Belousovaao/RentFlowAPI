namespace RentFlow.Application.RentalTariffs;

public sealed record RentalTariffDto(
    string Code,
    decimal Price,
    string Unit,
    DateTimeOffset UpdatedAtUtc);
