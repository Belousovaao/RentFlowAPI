using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Assets.Commands;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;
public class EnableAssetCommandHandler : IRequestHandler<EnableAssetCommand, Unit>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnableAssetCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EnableAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
            throw new NotFoundException($"Asset with id {request.Id} not found", _assetRepository);

        asset.Status = AssetStatus.Available;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}