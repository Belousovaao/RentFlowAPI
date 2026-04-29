using MediatR;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Handlers;
public class RemoveAssetPhotoCommandHandler : IRequestHandler<RemoveAssetPhotoCommand, Unit>
{
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAssetPhotoCommandHandler(IAssetRepository assetRepository, IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveAssetPhotoCommand request, CancellationToken ct)
    {
        Console.WriteLine($"=== RemoveAssetPhotoCommand ===");
        Console.WriteLine($"AssetId: {request.AssetId}");
        Console.WriteLine($"PhotoUrl: {request.PhotoUrl}");
        
        try
        {
            var asset = await _assetRepository.GetByIdAsync(request.AssetId, ct);
            if (asset == null)
            {
                Console.WriteLine("Asset not found!");
                throw new Exception("Asset not found");
            }
            
            Console.WriteLine($"Asset found. Before removal - photos count: {asset.Photos.Count}");
            
            // Удаляем фото из коллекции
            var photoToRemove = asset.Photos.FirstOrDefault(p => p.Url == request.PhotoUrl);
            if (photoToRemove != null)
            {
                // Используем рефлексию для доступа к приватному полю _photos
                var photosField = typeof(Asset).GetField("_photos", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var photosList = photosField?.GetValue(asset) as ICollection<AssetPhoto>;
                photosList?.Remove(photoToRemove);
                
                Console.WriteLine($"Photo removed from collection. After removal - photos count: {asset.Photos.Count}");
            }
            
            await _unitOfWork.SaveChangesAsync(ct);
            Console.WriteLine("Changes saved successfully");
            
            return Unit.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}