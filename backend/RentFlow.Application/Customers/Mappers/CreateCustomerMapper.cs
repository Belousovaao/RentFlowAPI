using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Application.Customers.Mappers;

public static class CreateCustomerMapper
{
    public static Passport? ToPassport(CustomerPassportDto? dto)
    {
        if (dto is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(dto.Serial) ||
        string.IsNullOrWhiteSpace(dto.Number))
        {
            return null;
        }

        return new Passport(
            dto.Serial,
            dto.Number,
            dto.IssuedBy,
            dto.IssuedDate,
            dto.RegistrationAddress);
    }

    public static DriverLicense? ToDriverLicense(DriverLicenseDto? dto)
    {
        if (dto is null)
            return null;

        if (string.IsNullOrWhiteSpace(dto.Number))
            return null;

        if (!dto.IssuedDate.HasValue ||
            !dto.ExpirationDate.HasValue)
        {
            return null;
        }

        return new DriverLicense(
            dto.Number,
            dto.Categories ?? [],
            dto.IssuedDate.Value,
            dto.ExpirationDate.Value,
            dto.ExperienceFrom);
    }

    public static BankAccount? ToBankAccount(BankInfoDto? dto)
    {
        if (dto is null)
            return null;

        if (string.IsNullOrWhiteSpace(dto.CurrentAccount) &&
            string.IsNullOrWhiteSpace(dto.CorrespondentAccount) &&
            string.IsNullOrWhiteSpace(dto.Bik) &&
            string.IsNullOrWhiteSpace(dto.BankName))
        {
            return null;
        }

        return new BankAccount(
            dto.CurrentAccount,
            dto.CorrespondentAccount,
            dto.Bik,
            dto.BankName);
    }

    public static OrganizationLegalForm ToLegalForm(CreateCustomerCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.LegalForm))
        {
            throw new ArgumentException("Legal form is required");
        }

        return OrganizationLegalForm.FromShortName(cmd.LegalForm);
    }
}