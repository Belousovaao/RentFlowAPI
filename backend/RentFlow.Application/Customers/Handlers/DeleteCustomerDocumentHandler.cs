using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;

public sealed class DeleteCustomerDocumentHandler : IRequestHandler<DeleteCustomerDocumentCommand>
{
    private readonly ICustomerRepository _customers;
    private readonly IFileStorage _storage;

    public DeleteCustomerDocumentHandler(
        ICustomerRepository customers,
        IFileStorage storage)
    {
        _customers = customers;
        _storage = storage;
    }

    public async Task Handle(
        DeleteCustomerDocumentCommand request,
        CancellationToken ct)
    {
        var document = await _customers.GetDocumentByIdAsync(
            request.CustomerId,
            request.DocumentId,
            ct);

        if (document is null)
            throw new NotFoundException("Customer document not found", request.DocumentId);

        await _storage.DeleteAsync(document.ObjectKey, ct);
        await _customers.DeleteDocumentAsync(document, ct);
    }
}