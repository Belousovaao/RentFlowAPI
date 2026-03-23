using System;
using MediatR;
using RentFlow.Application.Assets.Dtos;

namespace RentFlow.Application.Assets.Queries;

public sealed record SearchAssetsQuery(
    AssetFilter Filter) : IRequest<List<AssetDto>>;
