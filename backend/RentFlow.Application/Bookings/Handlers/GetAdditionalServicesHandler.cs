using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Bookings.Handlers;

public class GetAdditionalServicesHandler : IRequestHandler<GetAdditionalServicesQuery, List<AdditionalServiceDto>>
{
    private readonly IAdditionalServiceRepository _repo;

    public GetAdditionalServicesHandler(
        IAdditionalServiceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AdditionalServiceDto>> Handle(
        GetAdditionalServicesQuery request,
        CancellationToken ct)
    {
        var services = await _repo.GetActiveAsync(ct);

        return services
            .Select(x => new AdditionalServiceDto(
                x.Id,
                x.Name,
                x.Type,
                x.Price))
            .ToList();
    }
}