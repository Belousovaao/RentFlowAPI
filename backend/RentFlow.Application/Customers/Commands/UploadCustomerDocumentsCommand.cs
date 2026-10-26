using MediatR;

namespace RentFlow.Application.Customers.Commands;

public sealed record UploadCustomerDocumentCommand(
    Guid CustomerId,
    DocumentType Type,
    Stream FileStream,
    string FileName
) : IRequest<Guid>;