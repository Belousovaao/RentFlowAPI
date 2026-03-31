using MediatR;
using RentFlow.Application.Assets.Dtos;

namespace RentFlow.Application.Assets.Queries;

public sealed record GetAssetByCodeQuery(string Code) : IRequest<AssetDto?>;
