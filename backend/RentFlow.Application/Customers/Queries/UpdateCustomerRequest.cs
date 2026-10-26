using RentFlow.Application.Customers.Dtos;

namespace RentFlow.Application.Customers.Queries;
public sealed record UpdateCustomerRequest(
    string? FirstName,
    string? LastName,
    string? MiddleName,

    string? ShortName,
    string? FullName,
    string? LegalForm,

    string? Email,
    string? Phone,

    DateOnly? DateOfBirth,

    CustomerPassportDto? Passport,
    DriverLicenseDto? DriverLicense,

    string? Inn,
    string? Ogrnip,
    string? Ogrn,
    string? Kpp,

    string? OrganizationAddress,
    string? FactAddress,

    BankInfoDto? BankInfo,
    IReadOnlyList<CustomerDocumentDto>? Documents
);
