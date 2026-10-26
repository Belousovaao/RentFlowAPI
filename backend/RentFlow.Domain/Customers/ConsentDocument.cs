using System.Security.Cryptography;
using System.Text;

namespace RentFlow.Domain.Customers;

public sealed class ConsentDocument
{
    public Guid Id { get; private set; }
    public ConsentDocumentType Type { get; private set; }
    public string Version { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public string ContentHash { get; private set; } = null!;
    public DateTime PublishedAt { get; private set; }
    private ConsentDocument() { }

    public static ConsentDocument Publish(
        ConsentDocumentType type,
        string version,
        string title,
        string content,
        string contentHash,
        DateTime publishedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

        return new ConsentDocument
        {
            Id = Guid.NewGuid(),
            Type = type,
            Version = version.Trim(),
            Title = title.Trim(),
            Content = content,
            ContentHash = contentHash,
            PublishedAt = DateTime.SpecifyKind(
                publishedAt,
                DateTimeKind.Utc)
        };
    }

    public static ConsentDocument Publish(
        ConsentDocumentType type,
        string version,
        string title,
        string content,
        DateTime publishedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));

        if (publishedAt.Kind != DateTimeKind.Utc)
            throw new ArgumentException("PublishedAt must be UTC.", nameof(publishedAt));

        var contentHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(content)));

        return new ConsentDocument
        {
            Id = Guid.NewGuid(),
            Type = type,
            Version = version.Trim(),
            Title = title.Trim(),
            Content = content,
            ContentHash = contentHash,
            PublishedAt = publishedAt
        };
    }
}

public enum ConsentDocumentType
{
    PersonalDataPolicy = 1,
    PersonalDataProcessingConsent = 2
}