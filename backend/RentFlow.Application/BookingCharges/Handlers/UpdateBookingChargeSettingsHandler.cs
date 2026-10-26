using MediatR;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.BookingCharges.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class UpdateBookingChargeSettingsHandler : IRequestHandler<UpdateBookingChargeSettingsCommand, BookingChargeSettingsDto>
{
    private readonly IBookingChargeRepository _charges; private readonly IBookingRepository _bookings; private readonly IDateTimeProvider _clock;
    public UpdateBookingChargeSettingsHandler(IBookingChargeRepository charges, IBookingRepository bookings, IDateTimeProvider clock) { _charges = charges; _bookings = bookings; _clock = clock; }
    public async Task<BookingChargeSettingsDto> Handle(UpdateBookingChargeSettingsCommand request, CancellationToken ct)
    {
        var now = _clock.UtcNow; var settings = await _charges.GetSettingsAsync(ct) ?? BookingChargeSettings.CreateDefault(now);
        settings.Update(request.LateCancellationTariff, request.RefuelingPricePerLiter, now);
        await _charges.SaveSettingsAsync(settings, ct); await _bookings.SaveChangesAsync(ct);
        return new BookingChargeSettingsDto(settings.LateCancellationTariff, settings.RefuelingPricePerLiter, settings.UpdatedAt);
    }
}