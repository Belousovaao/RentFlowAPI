using RentFlow.Application.Bookings.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Dtos;
/// <summary>
/// Подробная информация о клиенте
/// </summary>
/// <param name="Id"></param>
/// <param name="Type"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="MiddleName"></param>
/// <param name="Email"></param>
/// <param name="Phone"></param>
/// <param name="DateOfBirth"></param>
/// <param name="VerificationStatus"></param>
/// <param name="Documents"></param>
/// <param name="Bookings"></param>
public sealed record CustomerDetailsDto(
    Guid Id,
    CustomerType Type,

    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? ShortName,
    string? FullName,
    string? LegalForm,

    string? Email,
    string? Phone,

    DateOnly? DateOfBirth,
    VerificationStatus VerificationStatus,
    CustomerPassportDto? Passport,
    DriverLicenseDto? DriverLicense,

    string? Inn,
    string? Ogrnip,
    string? Ogrn,
    string? Kpp,
    string? OrganizationAddress,
    string? FactAddress,

    BankInfoDto? BankInfo,      
    IReadOnlyList<CustomerDocumentDto> Documents,
    IReadOnlyList<BookingListItemDto> Bookings
);