using System;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Customer?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
    Task<List<CustomerLookupDto>> SearchAsync(CustomerType type, string query, CancellationToken ct);
    Task AddDocumentAsync(CustomerDocument document, CancellationToken ct);
    Task<IReadOnlyList<CustomerDocument>> GetDocumentsAsync(Guid customerId, CancellationToken ct);
    IQueryable<Customer> Query();
    Task<(IReadOnlyList<CustomerShortDto> Data, int TotalCount)> GetListAsync(
        int page,
        int pageSize,
        bool showRejected,
        string? search,
        CustomerType? type,
        CancellationToken ct
    );

    Task<CustomerDocument?> GetDocumentByIdAsync(
        Guid customerId,
        Guid documentId,
        CancellationToken ct);

    Task DeleteDocumentAsync(
        CustomerDocument document,
        CancellationToken ct);
}
