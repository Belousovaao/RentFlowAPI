using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Queries;

public sealed record GetActiveDestinationsQuery(
    DestinationType Type
) : IRequest<IReadOnlyList<DestinationDto>>;