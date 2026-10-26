using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Commands;
public sealed record CreateCustomerCommand(
    CustomerType Type,
    string? Email,
    string Phone,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    DateOnly? DateOfBirth,
    CustomerPassportDto? Passport,
    DriverLicenseDto? DriverLicense,

    // ИП
    string? INN,
    string? OGRNIP,

    // ООО
    string? FullName,
    string? ShortName,
    string? KPP,
    string? OGRN,

    // адреса
    string? OrganizationAddress,
    string? FactAddress,

    // банк
    BankInfoDto? BankInfo,

    // форма
    string? LegalForm
) : IRequest<CustomerLookupDto>;