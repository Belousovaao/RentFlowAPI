using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class GetActiveDestinationsHandler : IRequestHandler<GetActiveDestinationsQuery, IReadOnlyList<DestinationDto>>
{
    private readonly IDestinationRepository _repository;

    public GetActiveDestinationsHandler(IDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<DestinationDto>> Handle(
        GetActiveDestinationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Type is not DestinationType.Regional
            and not DestinationType.LongDistance)
        {
            throw new BusinessRuleViolationException("Only Regional and LongDistance destinations can be requested.");
        }

        var destinations = await _repository.GetActiveByTypeAsync(
            request.Type,
            cancellationToken);

        return destinations
            .Select(x => new DestinationDto(
                x.Id,
                x.Name,
                x.Region,
                x.DistanceKm,
                x.Type,
                x.RouteSurcharge))
            .ToList();
    }
}