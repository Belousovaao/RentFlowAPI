using MediatR;
using RentFlow.Application.Assets.Dtos;

namespace RentFlow.Application.Assets.Queries;
public class GetAssetByIdQuery : IRequest<AssetDto?>
{
    public Guid Id { get; set; }
    public GetAssetByIdQuery(Guid id) => Id = id;
}