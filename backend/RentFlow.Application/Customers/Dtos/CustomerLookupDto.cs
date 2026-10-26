using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Dtos;

public sealed record CustomerLookupDto(
    Guid Id,
    CustomerType Type,
    string DisplayName,
    string? Phone,
    string? Inn,
    string? Passport
);