using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Mappers;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Handlers;
public sealed class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IFileStorage _storage;
    public GetCustomerByIdHandler(ICustomerRepository customerRepository, IFileStorage fileStorage)
    {
        _customerRepo = customerRepository;
        _storage = fileStorage;
    }

    public async Task<CustomerDetailsDto> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var customer = await _customerRepo.GetByIdAsync(request.CustomerId, ct);
        if (customer is null)
            throw new NotFoundException("Customer not found", request.CustomerId);
        
        var customerDto = CustomerMapper.ToDetailsDto(customer);

        var documents = await _customerRepo.GetDocumentsAsync(request.CustomerId, ct);

        var documentDtos = new List<CustomerDocumentDto>(documents.Count);

        foreach (var document in documents)
        {
            var url = await _storage.CreatePresignedDownloadUrlAsync(document.ObjectKey);

            documentDtos.Add(new CustomerDocumentDto(
                document.Id,
                document.Type,
                url,
                document.UploadedAt));
        }

        return customerDto with
        {
            Documents = documentDtos
        };
    }
}