using MediatR;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Queries;
public class GetAssetByIdQuery : IRequest<Asset?>
{
    public Guid Id { get; set; }
    public GetAssetByIdQuery(Guid id) => Id = id;
}