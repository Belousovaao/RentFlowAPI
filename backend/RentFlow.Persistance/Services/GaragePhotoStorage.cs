using Amazon.S3.Util;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using RentFlow.Application.Interfaces;
using Amazon.S3;
using Amazon.Runtime;

namespace RentFlow.Persistance.Services;

public sealed class GaragePhotoStorage : IAssetPhotoStorage
{
    private readonly GarageOptions _options;
    private readonly IAmazonS3 _s3;

    public GaragePhotoStorage(IOptions<GarageOptions> options)
    {
        _options = options.Value;
        
        Console.WriteLine($"=== MinIO Configuration ===");
        Console.WriteLine($"Endpoint: {_options.Endpoint}");
        Console.WriteLine($"Bucket: {_options.Bucket}");
        Console.WriteLine($"AccessKey: {_options.AccessKey}");
        Console.WriteLine($"SecretKey: {new string('*', _options.SecretKey?.Length ?? 0)}");
        Console.WriteLine($"PublicBaseUrl: {_options.PublicBaseUrl}");
    
        var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
        
        var config = new AmazonS3Config
        {
            ServiceURL = _options.Endpoint,
            ForcePathStyle = true,
            UseHttp = true
        };

        _s3 = new AmazonS3Client(credentials, config);
    }

    public Task<(string ObjectKey, string UploadUrl)> CreatePresignedUploadAsync(
        Guid assetId,
        string fileName,
        string contentType,
        int expiresInSeconds = 900,
        CancellationToken ct = default)
    {
        var objectKey = BuildObjectKey(assetId, fileName);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(expiresInSeconds),
            ContentType = contentType
        };

        var uploadUrl = _s3.GetPreSignedURL(request);
        return Task.FromResult((objectKey, uploadUrl));
    }

    public async Task<bool> ExistsAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            await _s3.GetObjectMetadataAsync(_options.Bucket, objectKey, ct);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public string BuildPublicUrl(string objectKey)
    {
        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{objectKey}";
    }

    public async Task<string> UploadAsync(
        Guid assetId,
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var objectKey = BuildObjectKey(assetId, fileName);

        var putRequest = new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _s3.PutObjectAsync(putRequest, ct);

        return BuildPublicUrl(objectKey);
    }

    private static string BuildObjectKey(Guid assetId, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        return $"assets/{assetId}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
    }

    public async Task DeleteAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.Bucket,
                Key = objectKey
            };

            await _s3.DeleteObjectAsync(deleteRequest, ct);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Файл уже удален или не существует - игнорируем ошибку
        }
        catch (Exception ex)
        {
            // Пробрасываем исключение дальше, чтобы обработать на уровне выше
            throw new InvalidOperationException($"Failed to delete file {objectKey} from storage", ex);
        }
    }
}
