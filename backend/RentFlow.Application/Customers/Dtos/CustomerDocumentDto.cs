namespace RentFlow.Application.Customers.Dtos;
/// <summary>
/// Документ клиента
/// </summary>
/// <param name="Id"></param>
/// <param name="Type"></param>
/// <param name="Url"></param>
/// <param name="UploadedAt"></param>
public record CustomerDocumentDto(
    Guid Id,
    DocumentType Type,
    string Url,
    DateTime UploadedAt);