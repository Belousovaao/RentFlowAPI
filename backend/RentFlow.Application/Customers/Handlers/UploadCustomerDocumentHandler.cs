using MediatR;
using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;

public sealed class UploadCustomerDocumentHandler: IRequestHandler<UploadCustomerDocumentCommand, Guid>
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorage _storage;

    public UploadCustomerDocumentHandler(
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IFileStorage storage)
    {
        _customers = customers;
        _unitOfWork = unitOfWork;
        _storage = storage;
    }

    public async Task<Guid> Handle(UploadCustomerDocumentCommand request, CancellationToken ct)
    {
        var customer = await _customers.GetByIdAsync(request.CustomerId, ct);

        if (customer is null)
            throw new InvalidOperationException("Customer not found.");

        var extension = Path.GetExtension(request.FileName);

        if (string.IsNullOrWhiteSpace(extension))
            extension = ".jpg";

        var objectKey = $"customers/{request.CustomerId}/" + $"{request.Type.ToString().ToLowerInvariant()}{extension.ToLowerInvariant()}";

        await _storage.UploadAsync(objectKey, request.FileStream, GetContentType(extension), ct);

        CustomerDocument document = new CustomerDocument(
            request.CustomerId,
            request.Type,
            objectKey,
            DateTime.UtcNow);

        customer.AddDocument(document);
        await _customers.AddDocumentAsync(document, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return document.Id;
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
