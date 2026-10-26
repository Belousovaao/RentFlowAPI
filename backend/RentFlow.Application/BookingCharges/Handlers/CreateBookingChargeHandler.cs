using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class CreateBookingChargeHandler : IRequestHandler<CreateBookingChargeCommand, Guid>
{
    private readonly IBookingRepository _bookings; private readonly IBookingChargeRepository _charges; private readonly IDateTimeProvider _clock;
    public CreateBookingChargeHandler(IBookingRepository bookings, IBookingChargeRepository charges, IDateTimeProvider clock) { _bookings = bookings; _charges = charges; _clock = clock; }
    public async Task<Guid> Handle(CreateBookingChargeCommand request, CancellationToken ct)
    {
        if (request.BookingId == Guid.Empty) throw new ArgumentException("BookingId is required.");
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct) ?? throw new KeyNotFoundException("Booking not found.");
        if (request.Type == BookingChargeType.LateCancellation && booking.Status != BookingStatus.Cancelled)
            throw new InvalidOperationException("Late-cancellation charges can be added only to cancelled bookings.");
        var settings = await _charges.GetSettingsAsync(ct); var now = _clock.UtcNow;
        string title; string unit = "шт."; decimal quantity = 1m; decimal unitPrice; decimal amount;
        switch (request.Type)
        {
            case BookingChargeType.LateCancellation:
                title = "Поздняя отмена"; amount = settings?.LateCancellationTariff ?? 0m;
                if (amount <= 0) throw new InvalidOperationException("Сначала задайте тариф поздней отмены в настройках начислений.");
                unitPrice = amount; break;
            case BookingChargeType.Refueling:
                title = "Заправка"; quantity = request.Quantity ?? 0m;
                if (quantity <= 0 || quantity != decimal.Round(quantity, 3)) throw new ArgumentOutOfRangeException(nameof(request.Quantity));
                unit = "л"; unitPrice = settings?.RefuelingPricePerLiter ?? 0m;
                if (unitPrice <= 0) throw new InvalidOperationException("Сначала задайте цену за литр в настройках начислений.");
                amount = decimal.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero); break;
            case BookingChargeType.TrafficFine:
                title = "Штраф ГИБДД"; amount = request.Amount ?? 0m; unitPrice = amount;
                if (string.IsNullOrWhiteSpace(request.ReferenceNumber) && string.IsNullOrWhiteSpace(request.Description)) throw new ArgumentException("Для штрафа укажите номер постановления или описание.");
                break;
            case BookingChargeType.Cleaning:
                title = "Уборка"; amount = request.Amount ?? 0m; unitPrice = amount;
                if (string.IsNullOrWhiteSpace(request.Description)) throw new ArgumentException("Укажите описание работ по уборке.");
                break;
            case BookingChargeType.PropertyLossOrDamage:
                title = "Утрата имущества или повреждение"; amount = request.Amount ?? 0m; unitPrice = amount;
                if (string.IsNullOrWhiteSpace(request.Description)) throw new ArgumentException("Опишите повреждение или утраченное имущество.");
                break;
            case BookingChargeType.Other:
                title = request.Title?.Trim() ?? string.Empty; amount = request.Amount ?? 0m; unitPrice = amount;
                if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Для прочего начисления укажите название.");
                break;
            default: throw new ArgumentOutOfRangeException(nameof(request.Type));
        }
        if (amount <= 0 || amount != decimal.Round(amount, 2)) throw new ArgumentOutOfRangeException(nameof(request.Amount), "Сумма должна быть положительной и содержать не более двух знаков после запятой.");
        var charge = BookingCharge.Create(booking.Id, request.Type, title, request.Description, request.ReferenceNumber, quantity, unit, unitPrice, amount, now);
        await _charges.AddAsync(charge, ct); await _bookings.SaveChangesAsync(ct); return charge.Id;
    }
}