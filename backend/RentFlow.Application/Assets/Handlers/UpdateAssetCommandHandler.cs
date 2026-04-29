using MediatR;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers
{
    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Asset>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
        {
            _assetRepository = assetRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Asset> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
            if (asset == null)
                throw new Exception($"Asset with id {request.Id} not found");

            asset.UpdateBasicInfo(
                request.BrandName,
                request.Model,
                request.Year,
                request.DailyPrice,
                Enum.Parse<FuelType>(request.FuelType),
                Enum.Parse<TransmissionType>(request.Transmission),
                Enum.Parse<Domain.Assets.DriveType>(request.DriveType),
                request.Seats,
                request.Horsepower,
                Enum.Parse<AssetStatus>(request.Status)
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return asset;
        }
    }
}