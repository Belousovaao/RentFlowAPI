
using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Customers;
using System.Security.Cryptography;
using System.Text;

namespace RentFlow.Persistance.ConsentDocuments;

public sealed class ConsentDocumentSeeder
{
    private readonly RentFlowDbContext _db;

    public ConsentDocumentSeeder(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        const string relativePath =
            "ConsentDocuments/PersonalDataProcessingConsent_v1.0.md";

        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            relativePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Consent document file was not found.",
                filePath);
        }

        var content = await File.ReadAllTextAsync(
            filePath,
            cancellationToken);

        var version = "1.0";
        var title = "Согласие на обработку персональных данных";

        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(content)));

        var existing = await _db.Set<ConsentDocument>()
            .SingleOrDefaultAsync(
                d => d.Type ==
                         ConsentDocumentType.PersonalDataProcessingConsent
                     && d.Version == version,
                cancellationToken);

        if (existing is not null)
        {
            if (!string.Equals(
                    existing.ContentHash,
                    hash,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Published consent version 1.0 differs from the file. " +
                    "Do not overwrite a published document. Create a new version.");
            }

            return;
        }

        var document = ConsentDocument.Publish(
            ConsentDocumentType.PersonalDataProcessingConsent,
            version,
            title,
            content,
            DateTime.UtcNow);

        _db.Set<ConsentDocument>().Add(document);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
