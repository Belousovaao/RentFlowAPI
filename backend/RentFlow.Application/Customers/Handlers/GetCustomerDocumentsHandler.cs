using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;

public sealed class GetCustomerDocumentsHandler : IRequestHandler<GetCustomerDocumentsQuery, IReadOnlyList<CustomerDocumentDto>>
{
    private readonly ICustomerRepository _customers;
    private readonly IFileStorage _storage;

    public GetCustomerDocumentsHandler(ICustomerRepository customers, IFileStorage storage)
    {
        _customers = customers;
        _storage = storage;
    }

    public async Task<IReadOnlyList<CustomerDocumentDto>> Handle(GetCustomerDocumentsQuery request, CancellationToken ct)
    {
        var documents = await _customers.GetDocumentsAsync(request.CustomerId, ct);

        var result = new List<CustomerDocumentDto>();

        foreach (var document in documents)
        {
            var url = await _storage.CreatePresignedDownloadUrlAsync(document.ObjectKey);

            result.Add(new CustomerDocumentDto(
                document.Id,
                document.Type,
                url,
                document.UploadedAt));
        }

        return result;
    }
}