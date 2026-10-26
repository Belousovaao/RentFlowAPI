using MediatR;
using RentFlow.Application.Availability.Queries;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Availability.Handlers;
public class GetBusyPeriodsHandler : IRequestHandler<GetBusyPeriodsQuery, List<BusyPeriodDto>>
{
    private readonly IBookingRepository _bookings;

    public GetBusyPeriodsHandler(IBookingRepository bookings)
    {
        _bookings = bookings;
    }

    public async Task<List<BusyPeriodDto>> Handle(GetBusyPeriodsQuery request, CancellationToken ct)
    {
        return await _bookings.GetBusyPeriodsAsync(request.AssetId, request.ExcludeBookingId, ct);
    }
}