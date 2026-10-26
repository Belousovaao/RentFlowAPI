using System;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Persistance.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly RentFlowDbContext _context;

    public CustomerRepository(RentFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Customer?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _context.Customers.AddAsync(customer, ct);
    }

    public async Task<List<CustomerLookupDto>> SearchAsync(CustomerType type, string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        return type switch
        {
            CustomerType.Individual => await SearchIndividuals(query, ct),
            CustomerType.IndividualEntrepreneur => await SearchEntrepreneurs(query, ct),
            CustomerType.Organization => await SearchOrganizations(query, ct),
            _ => []
        };
    }

    private async Task<List<CustomerLookupDto>> SearchIndividuals(
        string query,
        CancellationToken ct)
    {
        var queryable = _context.Customers
            .AsNoTracking()
            .OfType<Individual>()
            .Where(x =>
                EF.Functions.ILike(x.Name.FirstName, $"%{query}%")
                || EF.Functions.ILike(x.Name.LastName, $"%{query}%")
                || EF.Functions.ILike(x.Name.MiddleName!, $"%{query}%")
                || EF.Functions.ILike(x.Phone, $"%{query}%")
            );

        return await queryable
            .Select(x => new CustomerLookupDto(
                x.Id,
                CustomerType.Individual,
                x.Name.FullName!,
                x.Phone,
                null,
                x.IndividualPassport == null
                    ? null
                    : $"{x.IndividualPassport.Serial} {x.IndividualPassport.Number}"
            ))
            .Take(20)
            .ToListAsync(ct);
    }

    private async Task<List<CustomerLookupDto>> SearchEntrepreneurs(string query, CancellationToken ct)
    {
        return await _context.Customers
            .AsNoTracking()
            .OfType<IndividualEntrepreneur>()
            .Where(x =>
                EF.Functions.ILike(x.Name.FirstName, $"%{query}%")
                || EF.Functions.ILike(x.Name.LastName, $"%{query}%")
                || EF.Functions.ILike(x.Name.MiddleName!, $"%{query}%")
                || EF.Functions.ILike(x.Phone, $"%{query}%")
                || EF.Functions.ILike(x.INN, $"%{query}%")
            )
            .Select(x => new CustomerLookupDto(
                x.Id,
                CustomerType.IndividualEntrepreneur,
                $"ИП {x.Name.FullName}",
                x.Phone,
                x.INN,
                null
            ))
            .Take(20)
            .ToListAsync(ct);
    }

    private async Task<List<CustomerLookupDto>> SearchOrganizations(string query, CancellationToken ct)
    {
        return await _context.Customers
            .AsNoTracking()
            .OfType<Organization>()
            .Where(x =>
                EF.Functions.ILike(x.ShortName, $"%{query}%")
                || EF.Functions.ILike(x.FullName, $"%{query}%")
                || EF.Functions.ILike(x.INN, $"%{query}%")
            )
            .Select(x => new CustomerLookupDto(
                x.Id,
                CustomerType.Organization,
                x.ShortName,
                x.Phone,
                x.INN,
                null
            ))
            .Take(20)
            .ToListAsync(ct);
    }

    public async Task AddDocumentAsync(CustomerDocument document, CancellationToken ct)
    {
        await _context.CustomerDocuments.AddAsync(document, ct);
    }

    public async Task<IReadOnlyList<CustomerDocument>> GetDocumentsAsync(Guid customerId, CancellationToken ct)
    {
        return await _context.CustomerDocuments
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Type)
            .ToListAsync(ct);
    }

    public IQueryable<Customer> Query()
    {
        return _context.Customers.AsNoTracking();
    }

    public async Task<(IReadOnlyList<CustomerShortDto> Data, int TotalCount)> GetListAsync(
        int page,
        int pageSize,
        bool showRejected,
        string? search,
        CustomerType? type,
        CancellationToken ct)
    {
        var customers = new List<CustomerShortDto>();

        // =========================
        // ФИЗИЧЕСКИЕ ЛИЦА
        // =========================
        if (!type.HasValue || type == CustomerType.Individual)
        {
            var query = _context.Customers.AsNoTracking().OfType<Individual>();

            if (!showRejected)
            {
                query = query.Where(x => x.VerificationStatus != VerificationStatus.Rejected);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                query = query.Where(x =>
                    EF.Functions.ILike(x.Name.FirstName, $"%{searchTerm}%") ||
                    EF.Functions.ILike(x.Name.LastName, $"%{searchTerm}%") ||
                    (x.Name.MiddleName != null &&
                    EF.Functions.ILike(x.Name.MiddleName, $"%{searchTerm}%")) ||
                    EF.Functions.ILike(x.Phone, $"%{searchTerm}%") ||
                    (x.Email != null &&
                    EF.Functions.ILike(x.Email, $"%{searchTerm}%"))
                );
            }

            var result = await query
                .Select(x => new CustomerShortDto(
                    x.Id,
                    CustomerType.Individual,
                    x.Name.FullName,
                    x.Email,
                    x.Phone,
                    x.VerificationStatus
                ))
                .ToListAsync(ct);
            customers.AddRange(result);
        }

        // =========================
        // ИП
        // =========================
        if (!type.HasValue || type == CustomerType.IndividualEntrepreneur)
        {
            var query = _context.Customers.AsNoTracking().OfType<IndividualEntrepreneur>();

            if (!showRejected)
            {
                query = query.Where(x => x.VerificationStatus != VerificationStatus.Rejected);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                query = query.Where(x =>
                    EF.Functions.ILike(x.Name.FirstName, $"%{searchTerm}%") ||
                    EF.Functions.ILike(x.Name.LastName, $"%{searchTerm}%") ||
                    (x.Name.MiddleName != null &&
                    EF.Functions.ILike(x.Name.MiddleName, $"%{searchTerm}%")) ||
                    EF.Functions.ILike(x.Phone, $"%{searchTerm}%") ||
                    (x.Email != null &&
                    EF.Functions.ILike(x.Email, $"%{searchTerm}%")) ||
                    EF.Functions.ILike(x.INN, $"%{searchTerm}%")
                );
            }

            var result = await query
                .Select(x => new CustomerShortDto(
                    x.Id,
                    CustomerType.IndividualEntrepreneur,
                    $"ИП {x.Name.FullName}",
                    x.Email,
                    x.Phone,
                    x.VerificationStatus
                ))
                .ToListAsync(ct);
            customers.AddRange(result);
        }

        // =========================
        // ОРГАНИЗАЦИИ
        // =========================
        if (!type.HasValue || type == CustomerType.Organization)
        {
            var query = _context.Customers.AsNoTracking().OfType<Organization>();

            if (!showRejected)
            {
                query = query.Where(x => x.VerificationStatus != VerificationStatus.Rejected);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();

                query = query.Where(x =>
                    EF.Functions.ILike(x.ShortName, $"%{searchTerm}%") ||
                    EF.Functions.ILike(x.FullName, $"%{searchTerm}%") ||
                    EF.Functions.ILike(x.Phone, $"%{searchTerm}%") ||
                    (x.Email != null &&
                    EF.Functions.ILike(x.Email, $"%{searchTerm}%")) ||
                    EF.Functions.ILike(x.INN, $"%{searchTerm}%")
                );
            }

            var result = await query
                .Select(x => new CustomerShortDto(
                    x.Id,
                    CustomerType.Organization,
                    x.ShortName ?? x.FullName,
                    x.Email,
                    x.Phone,
                    x.VerificationStatus
                ))
                .ToListAsync(ct);
            customers.AddRange(result);
        }

        // =========================
        // ОБЩАЯ СОРТИРОВКА
        // =========================
        customers = customers
            .OrderBy(x => x.DisplayName)
            .ToList();

        // =========================
        // ПАГИНАЦИЯ
        // =========================
        var totalCount = customers.Count;
        var pagedCustomers = customers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return (pagedCustomers, totalCount);
    }

    public async Task<CustomerDocument?> GetDocumentByIdAsync(
        Guid customerId,
        Guid documentId,
        CancellationToken ct)
    {
        return await _context.CustomerDocuments.FirstOrDefaultAsync(
            x => x.Id == documentId &&
                x.CustomerId == customerId,
            ct);
    }

    public async Task DeleteDocumentAsync(
        CustomerDocument document,
        CancellationToken ct)
    {
        _context.CustomerDocuments.Remove(document);
        await _context.SaveChangesAsync(ct);
    }

}
