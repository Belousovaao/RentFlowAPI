using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;

namespace RentFlow.Application.Customers.Commands;
public sealed record UpdateAdminCustomerCommand(
    Guid CustomerId,
    UpdateCustomerRequest Data
) : IRequest<CustomerDetailsDto>;