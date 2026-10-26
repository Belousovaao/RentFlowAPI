using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Queries;
public sealed record CreateCustomerRequest(
    CustomerType Type,

    string? Email,
    string Phone,

    string? FirstName,
    string? LastName,
    string? MiddleName,

    DateOnly? DateOfBirth,

    CustomerPassportDto? Passport,
    DriverLicenseDto? DriverLicense,

    string? Inn,
    string? Ogrnip,
    string? Ogrn,
    string? Kpp,

    string? FullName,
    string? ShortName,
    string? LegalForm,

    string? OrganizationAddress,
    string? FactAddress,

    BankInfoDto? BankInfo
);