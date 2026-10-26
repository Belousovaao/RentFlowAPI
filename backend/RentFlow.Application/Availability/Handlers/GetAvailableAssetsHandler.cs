using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Availability.Queries;
using RentFlow.Application.Assets.Mappers;

namespace RentFlow.Application.Availability.Handlers;
public class GetAvailableAssetsHandler : IRequestHandler<GetAvailableAssetsQuery, List<AssetDto>>
{
    private readonly IAssetRepository _assets;
    private readonly IBookingRepository _bookings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetAvailableAssetsHandler(
        IAssetRepository assets,
        IBookingRepository bookings,
        IDateTimeProvider dateTimeProvider)
    {
        _assets = assets;
        _bookings = bookings;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<List<AssetDto>> Handle(GetAvailableAssetsQuery request, CancellationToken ct)
    {
        var assets = await _assets.GetAllAsync();
        var startDate = _dateTimeProvider.ToUtc(request.StartDate, null);
        var endDate = _dateTimeProvider.ToUtc(request.EndDate, null);

        var bookedIds =
            await _bookings.GetBookedAssetIdsAsync(
                assets.Select(a => a.Id),
                startDate,
                endDate);

        return assets
            .Where(a => !bookedIds.Contains(a.Id))
            .Select(AssetMapper.ToDto)
            .ToList();
    }
}