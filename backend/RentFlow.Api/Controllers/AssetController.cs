using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Assets.Queries;
using MediatR;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAssetRepository _assetRepository;
        private readonly IFileStorage _assetPhotoStorage;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        public AssetsController (
            IMediator mediator,
            IAssetRepository assetRepository,
            IFileStorage assetPhotoStorage,
            IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _assetRepository = assetRepository;
            _assetPhotoStorage = assetPhotoStorage;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AssetFilter filter, CancellationToken ct)
        {
            var result = await _mediator.Send(new SearchAssetsQuery(filter), ct);
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByCodeQuery(code), ct);
            if (asset == null)
                return NotFound();

            return Ok(asset);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByIdQuery(id), ct);
            if (asset == null) return NotFound();
            return Ok(asset);
        }

        [HttpPost("{assetId:guid}/photos/upload-url")]
        public async Task<IActionResult> CreatePhotoUploadUrl(
            Guid assetId,
            [FromBody] CreateAssetPhotoUploadUrlRequest request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
                return BadRequest("FileName is required.");

            if (!AllowedImageTypes.Contains(request.ContentType))
                return BadRequest("Unsupported content type.");

            var asset = await _assetRepository.GetByIdAsync(assetId, ct);
            if (asset is null)
                return NotFound("Asset not found.");

            var (objectKey, uploadUrl) = await _assetPhotoStorage.CreatePresignedUploadAsync(
                assetId,
                request.FileName,
                request.ContentType,
                request.ExpiresInSeconds,
                ct);

            return Ok(new
            {
                assetId,
                objectKey,
                uploadUrl,
                publicUrl = _assetPhotoStorage.BuildPublicUrl(objectKey),
                expiresInSeconds = request.ExpiresInSeconds
            });
        }

        [HttpPost("{assetId:guid}/photos/complete")]
        public async Task<IActionResult> CompletePhotoUpload(
            Guid assetId,
            [FromBody] CompleteAssetPhotoUploadRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.ObjectKey))
                return BadRequest("ObjectKey is required.");

            if (!request.ObjectKey.StartsWith($"assets/{assetId}/", StringComparison.OrdinalIgnoreCase))
                return BadRequest("ObjectKey does not belong to this asset.");

            var asset = await _assetRepository.GetByIdAsync(assetId, ct);
            if (asset is null)
                return NotFound("Asset not found.");

            var exists = await _assetPhotoStorage.ExistsAsync(request.ObjectKey, ct);
            if (!exists)
                return BadRequest("Object not found in Garage.");

            var photoUrl = _assetPhotoStorage.BuildPublicUrl(request.ObjectKey);
            asset.AddPhoto(photoUrl, Domain.Assets.PhotoType.Exterior, 0);
            await _unitOfWork.SaveChangesAsync(ct);

            return Ok(new { assetId, photoUrl });
        }

        public sealed record CreateAssetPhotoUploadUrlRequest(
            string FileName,
            string ContentType,
            int ExpiresInSeconds = 900
        );

        public sealed record CompleteAssetPhotoUploadRequest(string ObjectKey);
    }
}
