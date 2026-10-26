using MediatR;

namespace RentFlow.Application.Customers.Commands;

public sealed record DeleteCustomerDocumentCommand(
    Guid CustomerId,
    Guid DocumentId
) : IRequest;