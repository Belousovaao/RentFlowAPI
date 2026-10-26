namespace RentFlow.Application.BookingCharges.Dtos;
public sealed record BookingChargeSettingsDto(decimal LateCancellationTariff, decimal RefuelingPricePerLiter, DateTimeOffset UpdatedAt);