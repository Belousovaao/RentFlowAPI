using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Interfaces;
using MediatR;
using System.Diagnostics;
using RentFlow.Domain.Assets;

namespace RentFlow.Api.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/assets")]
    public class AdminAssetsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAssetPhotoStorage _photoStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetRepository _assetRepository;

        public AdminAssetsController(IMediator mediator, IAssetPhotoStorage photoStorage, IUnitOfWork unitOfWork, IAssetRepository assetRepository)
        {
            _mediator = mediator;
            _photoStorage = photoStorage;
            _unitOfWork = unitOfWork;
            _assetRepository = assetRepository;
        }

        // GET: api/admin/assets (список для админа с фильтрацией)
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] AdminAssetFilter filter, CancellationToken ct)
        {
            Debug.WriteLine($"=== AdminAssetsController.GetList ===");
            Debug.WriteLine($"Filter: Page={filter.Page}, PageSize={filter.PageSize}");
            try
            {
                var result = await _mediator.Send(new GetAssetsForAdminQuery(filter), ct);
                Debug.WriteLine($"Result type: {result?.GetType()}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/admin/assets/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(id), ct);
            if (asset == null) return NotFound();
            return Ok(asset);
        }

        // POST: api/admin/assets
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssetCommand command, CancellationToken ct)
        {
            var asset = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }

        // PUT: api/admin/assets/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            var updated = await _mediator.Send(command, ct);
            return Ok(updated);
        }

        [HttpPatch("{id:guid}/disable")]
        public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DisableAssetCommand(id), ct);
            return Ok();
        }

        [HttpPatch("{id:guid}/enable")]
        public async Task<IActionResult> Enable(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new EnableAssetCommand(id), ct);
            return Ok();
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAssetStatusRequest request, CancellationToken ct)
        {
            await _mediator.Send(new UpdateAssetStatusCommand(id, request.Status), ct);
            return Ok();
        }

        [HttpPost("{assetId:guid}/photos/exterior/batch")]
        public async Task<IActionResult> UploadExteriorPhotos(Guid assetId, IFormFileCollection files, CancellationToken ct)
        {
            Console.WriteLine($"=== UploadExteriorPhotos START ===");
            Console.WriteLine($"AssetId: {assetId}");
            Console.WriteLine($"Files count: {files?.Count ?? 0}");
    
            if (files == null || files.Count == 0)
                return BadRequest("No files provided");
    
            List<object> result = new List<object>();
            Console.WriteLine("Getting asset...");
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset == null)
                return NotFound("Asset not found");

            Console.WriteLine($"Asset found. Current exterior photos: {asset.GetExteriorPhotos().Count}");
            int order = asset.GetExteriorPhotos().Count;
            Console.WriteLine($"Starting order: {order}");

            foreach (var file in files)
            {
                Console.WriteLine($"Processing file: {file.FileName}, Size: {file.Length}");
                if (file.Length == 0) continue;
        
                await using var stream = file.OpenReadStream();
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        
                Console.WriteLine($"Uploading to MinIO: {fileName}");
                var photoUrl = await _photoStorage.UploadAsync(assetId, stream, fileName, file.ContentType, ct);
                Console.WriteLine($"Uploaded to MinIO. URL: {photoUrl}");
                Console.WriteLine($"Sending UploadAssetPhotoCommand...");
                var photo = await _mediator.Send(new UploadAssetPhotoCommand(assetId, photoUrl, PhotoType.Exterior, order++), ct);
                Console.WriteLine($"Photo added to DB. Id: {photo?.Id}");

                result.Add(new { id = photo.Id, url = photoUrl});
            }
            Console.WriteLine($"=== UploadExteriorPhotos END. Successfully added {result.Count} photos ===");
            return Ok(new { urls = result, count = result.Count });
        }

        [HttpPost("{assetId:guid}/photos/exterior/reorder")]
        public async Task<IActionResult> ReorderExteriorPhotos(Guid assetId, [FromBody] ReorderPhotosRequest request, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset == null) return NotFound();
            
            // Обновляем порядок фото
            var exteriorPhotos = asset.GetExteriorPhotos().ToList();
            for (int i = 0; i < request.PhotoIds.Count; i++)
            {
                var photo = exteriorPhotos.FirstOrDefault(p => p.Id == request.PhotoIds[i]);
                if (photo != null)
                {
                    photo.UpdateOrder(i);
                }
            }
            
            await _unitOfWork.SaveChangesAsync(ct);
            return Ok();
        }

        [HttpPost("{assetId:guid}/photos/interior/reorder")]
        public async Task<IActionResult> ReorderEInteriorPhotos(Guid assetId, [FromBody] ReorderPhotosRequest request, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset == null) return NotFound();
            
            // Обновляем порядок фото
            var interiorPhotos = asset.GetInteriorPhotos().ToList();
            for (int i = 0; i < request.PhotoIds.Count; i++)
            {
                var photo = interiorPhotos.FirstOrDefault(p => p.Id == request.PhotoIds[i]);
                if (photo != null)
                {
                    photo.UpdateOrder(i);
                }
            }
            
            await _unitOfWork.SaveChangesAsync(ct);
            return Ok();
        }

        public record ReorderPhotosRequest(List<Guid> PhotoIds);

        [HttpPost("{assetId:guid}/photos/interior/batch")]
        public async Task<IActionResult> UploadInteriorPhotos(Guid assetId, IFormFileCollection files, CancellationToken ct)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files provided");
    
            var result = new List<object>();
            var order = 0;
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset != null)
            {
                order = asset.GetInteriorPhotos().Count;
            }
            foreach (var file in files)
            {
                if (file.Length == 0) continue;
        
                await using var stream = file.OpenReadStream();
                
                var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        
                var photoUrl = await _photoStorage.UploadAsync(assetId, stream, fileName, file.ContentType, ct);
                
                asset.AddPhoto(photoUrl, PhotoType.Interior, order++);
                await _unitOfWork.SaveChangesAsync(ct);
                
                var addedPhoto = asset.GetInteriorPhotos().LastOrDefault();
                
                result.Add(new { id = addedPhoto?.Id, url = photoUrl });
            }
            
            return Ok(new { urls = result, count = result.Count });
        }

        [HttpDelete("{assetId:guid}/photos/exterior/{photoId:guid}")]
        public async Task<IActionResult> DeleteExteriorPhoto(Guid assetId, Guid photoId, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset == null)
                return NotFound("Asset not found");
            
            var photo = asset.GetExteriorPhotos().FirstOrDefault(p => p.Id == photoId);
            if (photo == null)
                return NotFound("Photo not found");
            
            // Удаляем из MinIO
            var objectKey = ExtractObjectKeyFromUrl(photo.Url);
            if (!string.IsNullOrEmpty(objectKey))
                await _photoStorage.DeleteAsync(objectKey, ct);
            
            // Удаляем из БД
            await _mediator.Send(new RemoveAssetPhotoCommand(assetId, photo.Url), ct);
            
            return NoContent();
        }

        // Удаление одного фото интерьера
        [HttpDelete("{assetId:guid}/photos/interior/{photoId:guid}")]
        public async Task<IActionResult> DeleteInteriorPhoto(Guid assetId, Guid photoId, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(assetId), ct);
            if (asset == null)
                return NotFound("Asset not found");
            
            var photo = asset.GetInteriorPhotos().FirstOrDefault(p => p.Id == photoId);
            if (photo == null)
                return NotFound("Photo not found");
            
            // Удаляем из MinIO
            var objectKey = ExtractObjectKeyFromUrl(photo.Url);
            if (!string.IsNullOrEmpty(objectKey))
                await _photoStorage.DeleteAsync(objectKey, ct);
            
            // Удаляем из БД
            await _mediator.Send(new RemoveAssetPhotoCommand(assetId, photo.Url), ct);
            
            return NoContent();
        }


        private string? ExtractObjectKeyFromUrl(string photoUrl)
        {
            // В зависимости от реализации BuildPublicUrl, вытаскиваем путь после bucket/ или просто относительный путь
            var uri = new Uri(photoUrl);
            return uri.AbsolutePath.TrimStart('/');
        }

        public record UpdateAssetStatusRequest(string Status);


    }
}