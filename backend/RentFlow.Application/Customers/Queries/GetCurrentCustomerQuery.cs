using MediatR;
using RentFlow.Application.Customers.Dtos;

namespace RentFlow.Application.Customers.Queries;

public sealed record GetCurrentCustomerQuery(Guid UserId) : IRequest<CustomerDetailsDto>;