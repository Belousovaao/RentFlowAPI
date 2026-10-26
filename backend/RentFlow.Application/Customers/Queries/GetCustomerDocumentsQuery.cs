using MediatR;
using RentFlow.Application.Customers.Dtos;

namespace RentFlow.Application.Customers.Queries;

public sealed record GetCustomerDocumentsQuery(
    Guid CustomerId
) : IRequest<IReadOnlyList<CustomerDocumentDto>>;