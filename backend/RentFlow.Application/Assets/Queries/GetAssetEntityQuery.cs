using MediatR;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Queries;
public sealed record GetAssetEntityQuery(Guid Id)
    : IRequest<Asset?>;