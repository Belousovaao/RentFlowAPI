using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Queries;
public class GetAssetsForAdminQuery : IRequest<PaginatedResponse<Asset>>
{
    public AdminAssetFilter Filter { get; set; }
    
    public GetAssetsForAdminQuery(AdminAssetFilter adminAssetFilter)
    {
        Filter = adminAssetFilter;
    }
}