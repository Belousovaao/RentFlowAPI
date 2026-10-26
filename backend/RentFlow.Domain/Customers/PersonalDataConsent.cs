namespace RentFlow.Domain.Customers;

public enum ConsentGrantMethod
{
    WebCheckbox = 1
}

public sealed class PersonalDataConsent
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid ConsentDocumentId { get; private set; }

    public DateTime GrantedAt { get; private set; }

    public ConsentGrantMethod Method { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public ConsentDocument Document { get; private set; } = null!;

    private PersonalDataConsent() { }

    public static PersonalDataConsent Grant(
        Guid customerId,
        ConsentDocument document,
        DateTime grantedAt,
        ConsentGrantMethod method)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "CustomerId is required.",
                nameof(customerId));

        if (document.Type !=
            ConsentDocumentType.PersonalDataProcessingConsent)
        {
            throw new ArgumentException(
                "The document is not a processing consent.",
                nameof(document));
        }

        if (grantedAt.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "GrantedAt must be UTC.",
                nameof(grantedAt));
        }

        if (!Enum.IsDefined(method))
            throw new ArgumentOutOfRangeException(nameof(method));

        return new PersonalDataConsent
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ConsentDocumentId = document.Id,
            Document = document,
            GrantedAt = grantedAt,
            Method = method
        };
    }
}