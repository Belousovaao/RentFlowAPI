using MediatR;
using RentFlow.Application.Assets.Dtos;

namespace RentFlow.Application.Availability.Queries;

public sealed record GetAvailableAssetsQuery(
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest<List<AssetDto>>;