using System;
using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Assets.Handlers;

public sealed class GetAssetByCodeHandler : IRequestHandler<GetAssetByCodeQuery, AssetDto?>
{
    private readonly IAssetRepository _repo;
    public GetAssetByCodeHandler(IAssetRepository repo)
    {
        _repo = repo;
    }
    public async Task<AssetDto?> Handle(GetAssetByCodeQuery request, CancellationToken cancellationToken)
    {
        var asset = await _repo.GetByCodeAsync(request.Code, cancellationToken);

        if (asset == null)
            return null;

        return new AssetDto(
            asset.Id,
            asset.Code,
            asset.BrandName,
            asset.Model,
            asset.DailyPrice
        );
    }
}
