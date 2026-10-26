namespace RentFlow.Api.Services.Requests;

public class UploadCustomerDocumentsRequest
{
    public DocumentType Type { get; set; }

    public IFormFile File { get; set; } = null!;
}