using MediatR;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;

public sealed class GetAssetEntityHandler
    : IRequestHandler<GetAssetEntityQuery, Asset?>
{
    private readonly IAssetRepository _repository;

    public GetAssetEntityHandler(IAssetRepository repository)
    {
        _repository = repository;
    }

    public Task<Asset?> Handle(
        GetAssetEntityQuery request,
        CancellationToken ct)
    {
        return _repository.GetByIdAsync(request.Id, ct);
    }
}