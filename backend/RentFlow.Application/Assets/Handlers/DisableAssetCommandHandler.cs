using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Application.Assets.Commands;

namespace RentFlow.Application.Assets.Handlers;
public class DisableAssetCommandHandler : IRequestHandler<DisableAssetCommand, Unit>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DisableAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
            throw new NotFoundException($"Asset with id {request.Id} not found", _assetRepository);

        asset.Status = AssetStatus.Disabled;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}