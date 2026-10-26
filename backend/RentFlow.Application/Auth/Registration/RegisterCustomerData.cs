using RentFlow.Domain.Customers;

namespace RentFlow.Application.Auth.Registration;

public sealed record RegisterCustomerData(
    CustomerType CustomerType,
    string? Email,
    string Phone,
    string? FirstName,
    string? LastName,
    string? Inn,
    Guid ConsentDocumentId,
    bool PersonalDataConsentAccepted);