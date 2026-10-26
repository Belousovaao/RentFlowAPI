
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Customers;
using RentFlow.Persistance;

namespace RentFlow.Api.Controllers;

[ApiController]
[Route("api/consent-documents")]
public sealed class ConsentDocumentsController : ControllerBase
{
    private readonly RentFlowDbContext _db;

    public ConsentDocumentsController(RentFlowDbContext db)
    {
        _db = db;
    }

    [HttpGet("personal-data-processing")]
    public async Task<IActionResult> GetPersonalDataProcessingConsent(
        CancellationToken cancellationToken)
    {
        var document = await _db.Set<ConsentDocument>()
            .AsNoTracking()
            .Where(d =>
                d.Type == ConsentDocumentType.PersonalDataProcessingConsent)
            .OrderByDescending(d => d.PublishedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (document is null)
        {
            return NotFound(new
            {
                message = "Personal data processing consent is not published."
            });
        }

        return Ok(new
        {
            document.Id,
            document.Version,
            document.Title,
            document.Content,
            document.ContentHash,
            document.PublishedAt
        });
    }
}
