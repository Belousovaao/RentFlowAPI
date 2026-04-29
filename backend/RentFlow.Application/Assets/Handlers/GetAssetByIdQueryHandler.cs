using MediatR;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Queries
{
    public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, Asset?>
    {
        private readonly IAssetRepository _assetRepository;
        
        public GetAssetByIdQueryHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }
        
        public async Task<Asset?> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
        {
            return await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}