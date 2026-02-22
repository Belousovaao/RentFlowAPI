using System;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
}
