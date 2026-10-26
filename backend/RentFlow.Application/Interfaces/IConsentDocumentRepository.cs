using RentFlow.Domain.Customers;

namespace RentFlow.Application.Interfaces;

public interface IConsentDocumentRepository
{
    Task<ConsentDocument?> GetProcessingConsentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}