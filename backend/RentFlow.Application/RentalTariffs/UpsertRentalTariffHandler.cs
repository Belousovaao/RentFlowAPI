using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Settings;

namespace RentFlow.Application.RentalTariffs;

public sealed class UpsertRentalTariffHandler : IRequestHandler<UpsertRentalTariffCommand, RentalTariffDto>
{
    private readonly IRentalTariffRepository _tariffs;
    private readonly IBookingRepository _unitOfWork;
    private readonly IDateTimeProvider _clock;

    public UpsertRentalTariffHandler(
        IRentalTariffRepository tariffs,
        IBookingRepository unitOfWork,
        IDateTimeProvider clock)
    {
        _tariffs = tariffs;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<RentalTariffDto> Handle(UpsertRentalTariffCommand request, CancellationToken ct)
    {
        var code = request.Code?.Trim() ?? string.Empty;
        var unit = request.Unit?.Trim() ?? string.Empty;
        var tariff = await _tariffs.GetByCodeAsync(code, ct);

        if (tariff is null)
        {
            tariff = RentalTariff.Create(code, request.Price, unit, _clock.UtcNow);
            await _tariffs.AddAsync(tariff, ct);
        }
        else
        {
            if (!string.Equals(tariff.Unit, unit, StringComparison.Ordinal))
                throw new InvalidOperationException("A tariff unit cannot be changed after the tariff is created.");

            tariff.UpdatePrice(request.Price, _clock.UtcNow);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return new RentalTariffDto(tariff.Code, tariff.Price, tariff.Unit, tariff.UpdatedAtUtc);
    }
}
