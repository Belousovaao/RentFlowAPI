using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Domain.Settings;

namespace RentFlow.Application.BookingCharges.Handlers;

public sealed class CreateBookingChargeHandler : IRequestHandler<CreateBookingChargeCommand, Guid>
{
    private readonly IBookingRepository _bookings;
    private readonly IBookingChargeRepository _charges;
    private readonly IRentalTariffRepository _tariffs;
    private readonly IDateTimeProvider _clock;

    public CreateBookingChargeHandler(
        IBookingRepository bookings,
        IBookingChargeRepository charges,
        IRentalTariffRepository tariffs,
        IDateTimeProvider clock)
    {
        _bookings = bookings;
        _charges = charges;
        _tariffs = tariffs;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateBookingChargeCommand request, CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty)
            throw new ArgumentException("BookingId is required.");

        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new KeyNotFoundException("Booking not found.");

        var now = _clock.UtcNow;
        string title;
        string unit = "шт.";
        decimal quantity = 1m;
        decimal unitPrice;
        decimal amount;

        switch (request.Type)
        {
            case BookingChargeType.LateCancellation:
                if (booking.Status != BookingStatus.Cancelled)
                    throw new InvalidOperationException("Late-cancellation charges can be added only to cancelled bookings.");

                title = "Поздняя отмена";
                unitPrice = booking.Price.RentalPrice.LastSegment.BaseDailyPrice;
                amount = unitPrice;
                break;

            case BookingChargeType.Refueling:
                title = "Заправка";
                quantity = request.Quantity ?? 0m;
                if (quantity <= 0 || quantity != decimal.Round(quantity, 3))
                    throw new ArgumentOutOfRangeException(nameof(request.Quantity));

                unit = "л";
                unitPrice = await GetRequiredTariffPrice(RentalTariffCodes.FuelPerLiter, ct);
                amount = decimal.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);
                break;

            case BookingChargeType.ExcessMileage:
                title = "Перепробег";
                quantity = request.Quantity ?? 0m;
                if (quantity <= 0 || quantity != decimal.Round(quantity, 3))
                    throw new ArgumentOutOfRangeException(nameof(request.Quantity));

                unit = "км";
                unitPrice = await GetRequiredTariffPrice(RentalTariffCodes.ExcessMileagePerKm, ct);
                amount = decimal.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);
                break;

            case BookingChargeType.Cleaning:
                title = "Мойка кузова";
                if (string.IsNullOrWhiteSpace(request.Description))
                    throw new ArgumentException("Укажите описание работ по уборке.");

                unitPrice = request.Amount.HasValue
                    ? request.Amount.Value
                    : await GetRequiredTariffPrice(RentalTariffCodes.BodyWash, ct);
                amount = unitPrice;
                break;

            case BookingChargeType.TrafficFine:
                title = "Штраф ГИБДД";
                amount = request.Amount ?? 0m;
                unitPrice = amount;
                if (string.IsNullOrWhiteSpace(request.ReferenceNumber) &&
                    string.IsNullOrWhiteSpace(request.Description))
                    throw new ArgumentException("Для штрафа укажите номер постановления или описание.");
                break;

            case BookingChargeType.PropertyLossOrDamage:
                title = "Утрата имущества или повреждение";
                amount = request.Amount ?? 0m;
                unitPrice = amount;
                if (string.IsNullOrWhiteSpace(request.Description))
                    throw new ArgumentException("Опишите повреждение или утраченное имущество.");
                break;

            case BookingChargeType.Other:
                title = request.Title?.Trim() ?? string.Empty;
                amount = request.Amount ?? 0m;
                unitPrice = amount;
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Для прочего начисления укажите название.");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(request.Type));
        }

        if (amount <= 0 || amount != decimal.Round(amount, 2))
            throw new ArgumentOutOfRangeException(
                nameof(request.Amount),
                "Сумма должна быть положительной и содержать не более двух знаков после запятой.");

        var charge = BookingCharge.Create(
            booking.Id,
            request.Type,
            title,
            request.Description,
            request.ReferenceNumber,
            quantity,
            unit,
            unitPrice,
            amount,
            now);

        await _charges.AddAsync(charge, ct);
        await _bookings.SaveChangesAsync(ct);
        return charge.Id;
    }

    private async Task<decimal> GetRequiredTariffPrice(string code, CancellationToken ct)
    {
        var tariff = await _tariffs.GetByCodeAsync(code, ct);
        if (tariff is null || tariff.Price <= 0)
            throw new InvalidOperationException($"Не настроен тариф '{code}' в разделе тарифов аренды.");

        return tariff.Price;
    }
}
