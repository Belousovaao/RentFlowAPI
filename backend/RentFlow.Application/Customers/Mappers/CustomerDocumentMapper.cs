using RentFlow.Application.Customers.Dtos;

namespace RentFlow.Application.Customers.Mappers;

public static class CustomerDocumentMapper
{
    public static CustomerDocumentDto ToDto(CustomerDocument document)
    {
        return new CustomerDocumentDto(
            document.Id,
            document.Type,
            document.ObjectKey,
            document.UploadedAt
        );
    }
}