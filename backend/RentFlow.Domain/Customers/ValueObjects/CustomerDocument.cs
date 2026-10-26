using RentFlow.Domain.Customers;

public class CustomerDocument
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public DocumentType Type { get; private set; }

    public string ObjectKey { get; private set; }

    public DateTime UploadedAt { get; private set; }

    private CustomerDocument() { }

    public CustomerDocument(
        Guid customerId,
        DocumentType type,
        string objectKey,
        DateTime uploadedAt)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Type = type;
        ObjectKey = objectKey;
        UploadedAt = uploadedAt;
    }
}

public enum DocumentType
{
    PassportFront,
    PassportRegistration,
    DriverLicenseFront,
    DriverLicenseBack
}