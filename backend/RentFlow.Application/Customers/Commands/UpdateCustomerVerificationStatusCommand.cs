using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Commands;

public sealed record UpdateCustomerVerificationStatusCommand(
    Guid CustomerId,
    VerificationStatus Status
) : IRequest<CustomerDetailsDto>;