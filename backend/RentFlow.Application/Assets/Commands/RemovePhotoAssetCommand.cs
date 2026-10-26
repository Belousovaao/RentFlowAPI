using MediatR;

namespace RentFlow.Application.Assets.Commands;
public class RemoveAssetPhotoCommand : IRequest<Unit>
{
    public Guid AssetId { get; set; }
    public Guid PhotoId { get; set; }
    public RemoveAssetPhotoCommand(Guid assetId, Guid photoId) => (AssetId, PhotoId) = (assetId, photoId);
}