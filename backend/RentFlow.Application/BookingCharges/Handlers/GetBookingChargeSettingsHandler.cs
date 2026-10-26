using MediatR;
using RentFlow.Application.BookingCharges.Dtos;
using RentFlow.Application.BookingCharges.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
namespace RentFlow.Application.BookingCharges.Handlers;
public sealed class GetBookingChargeSettingsHandler : IRequestHandler<GetBookingChargeSettingsQuery, BookingChargeSettingsDto>
{
    private readonly IBookingChargeRepository _charges; private readonly IDateTimeProvider _clock;
    public GetBookingChargeSettingsHandler(IBookingChargeRepository charges, IDateTimeProvider clock) { _charges = charges; _clock = clock; }
    public async Task<BookingChargeSettingsDto> Handle(GetBookingChargeSettingsQuery request, CancellationToken ct)
    {
        var settings = await _charges.GetSettingsAsync(ct) ?? BookingChargeSettings.CreateDefault(_clock.UtcNow);
        return new BookingChargeSettingsDto(settings.LateCancellationTariff, settings.RefuelingPricePerLiter, settings.UpdatedAt);
    }
}