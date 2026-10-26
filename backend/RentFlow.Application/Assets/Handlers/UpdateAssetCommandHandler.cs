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

            asset.Update(
                request.BrandName,
                request.Model,
                request.ShortDescription,
                request.FullDescription,
                request.Category,
                request.Year,
                request.FuelType,
                request.Transmission,
                request.DriveType,
                request.Seats,
                request.Doors,
                request.Engine,
                request.Horsepower,
                request.Acceleration,
                request.TopSpeed,
                request.Color,
                request.Features,
                request.DailyPrice,
                request.Deposit,
                Enum.Parse<AssetStatus>(request.Status),
                request.Latitude,
                request.Longitude
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return asset;
        }
    }
}