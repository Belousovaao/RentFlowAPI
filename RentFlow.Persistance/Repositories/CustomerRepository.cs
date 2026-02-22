using System;
using Microsoft.EntityFrameworkCore;
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
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _context.Customers.AddAsync(customer, ct);
    }
}
