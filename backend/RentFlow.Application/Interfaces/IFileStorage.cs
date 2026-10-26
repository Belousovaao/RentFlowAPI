namespace RentFlow.Application.Interfaces;

public interface IFileStorage
{
    Task<(string ObjectKey, string UploadUrl)> CreatePresignedUploadAsync(
        Guid assetId,
        string fileName,
        string contentType,
        int expiresInSeconds = 900,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(string objectKey, CancellationToken ct = default);

    string BuildPublicUrl(string objectKey);

    Task<string> UploadAsync(
        Guid assetId,
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken ct = default);

    Task<string> UploadAsync(
        string objectKey,
        Stream fileStream,
        string contentType,
        CancellationToken ct = default);
    Task DeleteAsync(string objectKey, CancellationToken ct);

    Task<string> CreatePresignedDownloadUrlAsync(
        string objectKey,
        int expiresInSeconds = 900,
        CancellationToken ct = default);

}
