using RentFlow.Domain.Customers;

namespace RentFlow.Api.Services.Requests;

public sealed record RegisterRequest(
    string RegistrationTicket,
    CustomerType CustomerType,
    string? Email,
    string Phone,
    string? FirstName,
    string? LastName,
    string? Inn,
    Guid ConsentDocumentId,
    bool PersonalDataConsentAccepted);