using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Assets.Mappers;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Assets.Queries
{
    public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDto?>
    {
        private readonly IAssetRepository _assetRepository;
        
        public GetAssetByIdQueryHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }
        
        public async Task<AssetDto?> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
            return asset != null ? AssetMapper.ToDto(asset) : null;
        }
    }
}