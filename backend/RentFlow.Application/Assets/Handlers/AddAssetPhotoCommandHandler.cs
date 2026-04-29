using MediatR;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;

public class AddAssetPhotoCommandHandler : IRequestHandler<UploadAssetPhotoCommand, AssetPhoto>
{
    private readonly IAssetPhotoRepository _photoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddAssetPhotoCommandHandler(IAssetPhotoRepository photoRepository, IUnitOfWork unitOfWork)
    {
        _photoRepository = photoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AssetPhoto> Handle(UploadAssetPhotoCommand request, CancellationToken ct)
    {
        Console.WriteLine($"=== AddAssetPhotoCommandHandler START ===");
        try
        {
            Console.WriteLine("Adding photo to IAssetPhotoStorage...");
            var photo = new AssetPhoto(request.AssetId, request.PhotoUrl, request.Type, request.Order);

            Console.WriteLine("Saving changes to database...");
            await _photoRepository.AddAsync(photo, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            Console.WriteLine("Changes saved successfully");

            Console.WriteLine($"=== AddAssetPhotoCommandHandler END ===");
            
            return photo!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in AddAssetPhotoCommandHandler: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}