using MediatR;

namespace RentFlow.Application.Assets.Commands;
public class RemoveAssetPhotoCommand : IRequest<Unit>
{
    public Guid AssetId { get; set; }
    public string PhotoUrl { get; set; }
    public RemoveAssetPhotoCommand(Guid assetId, string photoUrl) => (AssetId, PhotoUrl) = (assetId, photoUrl);
}