using System;
using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class SearchAssetsHandler : IRequestHandler<SearchAssetsQuery, List<AssetDto>>
{
    private readonly IAssetRepository _repo;
    public SearchAssetsHandler(IAssetRepository repo)
    {
        _repo = repo;
    }
    public async Task<List<AssetDto>> Handle(SearchAssetsQuery request, CancellationToken cancellationToken)
    {
        var assets = await _repo.SearchAsync(request.Filter);
        return assets.Select(a => new AssetDto(
        a.Id,
        a.Code,
        a.BrandName,
        a.Model,
        a.DailyPrice)).ToList();
    }
}
