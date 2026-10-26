using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Repositories.Customers;

public sealed class ConsentDocumentRepository : IConsentDocumentRepository
{
    private readonly RentFlowDbContext _dbContext;

    public ConsentDocumentRepository(RentFlowDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public Task<ConsentDocument?> GetProcessingConsentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<ConsentDocument>().SingleOrDefaultAsync(document =>
            document.Id == id &&
            document.Type == ConsentDocumentType.PersonalDataProcessingConsent,
            cancellationToken);
    }


}