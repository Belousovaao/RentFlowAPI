using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Application.Assets.Commands;

namespace RentFlow.Application.Assets.Handlers;
public class UpdateAssetStatusCommandHandler : IRequestHandler<UpdateAssetStatusCommand, Unit>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAssetStatusCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateAssetStatusCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
            throw new NotFoundException($"Asset with id {request.Id} not found", _assetRepository);

        // Преобразуем строку в enum
        asset.Status = Enum.Parse<AssetStatus>(request.Status);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}