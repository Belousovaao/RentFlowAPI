using MediatR;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Commands;

public record UploadAssetPhotoCommand(
    Guid AssetId,
    string PhotoUrl,
    PhotoType Type,
    int Order
) : IRequest<AssetPhoto>;