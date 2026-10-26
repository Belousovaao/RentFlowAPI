using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Queries;

public sealed record SearchCustomersQuery(CustomerType Type, string Query) : IRequest<List<CustomerLookupDto>>;