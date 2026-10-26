using MediatR;
using RentFlow.Application.BookingCharges.Dtos;
namespace RentFlow.Application.BookingCharges.Commands;
public sealed record UpdateBookingChargeSettingsCommand(decimal LateCancellationTariff, decimal RefuelingPricePerLiter) : IRequest<BookingChargeSettingsDto>;