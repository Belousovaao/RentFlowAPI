using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Application.Customers.Mappers;

public static class CustomerMapper
{
    public static CustomerLookupDto ToLookup(Customer customer)
    {
        return customer switch
        {
            Individual i => new CustomerLookupDto(
                i.Id,
                CustomerType.Individual,
                i.Name.FullName!,
                i.Phone,
                null,
                i.IndividualPassport == null ? null : $"{i.IndividualPassport.Serial} {i.IndividualPassport.Number}"
            ),

            IndividualEntrepreneur ie => new CustomerLookupDto(
                ie.Id,
                CustomerType.IndividualEntrepreneur,
                $"ИП {ie.Name.FullName}",
                ie.Phone,
                ie.INN,
                null
            ),

            Organization o => new CustomerLookupDto(
                o.Id,
                CustomerType.Organization,
                o.ShortName,
                o.Phone,
                o.INN,
                null
            ),

            _ => throw new NotSupportedException()
        };
    }

    public static CustomerDetailsDto ToDetailsDto(Customer customer)
    {
        return customer switch
        {
            Individual i => new CustomerDetailsDto(
                i.Id,
                CustomerType.Individual,
                i.Name.FirstName,
                i.Name.LastName,
                i.Name.MiddleName,
                null,
                null,
                null,
                i.Email,
                i.Phone,
                i.DateOfBirth,
                i.VerificationStatus,
                i.IndividualPassport == null 
                    ? null 
                    : new CustomerPassportDto(
                        i.IndividualPassport.Serial,
                        i.IndividualPassport.Number,
                        i.IndividualPassport.IssuedBy,
                        i.IndividualPassport.IssuedDate,
                        i.IndividualPassport.RegistrationAddress
                    ),
                i.IndividualDriverLicense == null
                    ? null
                    : new DriverLicenseDto(
                        i.IndividualDriverLicense.Number,
                        i.IndividualDriverLicense.Categories,
                        i.IndividualDriverLicense.IssuedDate,
                        i.IndividualDriverLicense.ExpirationDate,
                        i.IndividualDriverLicense.ExprerienceFromYear
                    ),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                i.Documents
                    .Select(CustomerDocumentMapper.ToDto)
                    .ToList(),

                []
            ),

            Organization o => new CustomerDetailsDto(
                o.Id,
                CustomerType.Organization,
                null,
                null,
                null,
                o.ShortName,
                o.FullName,
                o.LegalForm.ShortName.ToString(),
                o.Email,
                o.Phone,
                null,
                o.VerificationStatus,
                null,
                null,
                o.INN,
                null,
                o.OGRN,
                o.KPP,
                o.OrganizationAddress,
                o.FactAddress,
                o.OrganizationBankAccount == null
                    ? null
                    : new BankInfoDto(
                        o.OrganizationBankAccount.CurrentAccount,
                        o.OrganizationBankAccount.CorrespondentAccount,
                        o.OrganizationBankAccount.BIK,
                        o.OrganizationBankAccount.BankName
                    ),

                o.Documents
                    .Select(CustomerDocumentMapper.ToDto)
                    .ToList(),

                []
            ),
            IndividualEntrepreneur ie => new CustomerDetailsDto(
                ie.Id,
                CustomerType.IndividualEntrepreneur,
                ie.Name.FirstName,
                ie.Name.LastName,
                ie.Name.MiddleName,
                null,
                null,
                ie.IpLegalForm.ToString(),
                ie.Email,
                ie.Phone,
                null,
                ie.VerificationStatus,
                ie.IPPassport == null 
                    ? null 
                    : new CustomerPassportDto(
                        ie.IPPassport.Serial,
                        ie.IPPassport.Number,
                        ie.IPPassport.IssuedBy,
                        ie.IPPassport.IssuedDate,
                        ie.IPPassport.RegistrationAddress),
                ie.EntrepreneurDriverLicense == null
                    ? null
                    : new DriverLicenseDto(
                        ie.EntrepreneurDriverLicense.Number,
                        ie.EntrepreneurDriverLicense.Categories,
                        ie.EntrepreneurDriverLicense.IssuedDate,
                        ie.EntrepreneurDriverLicense.ExpirationDate,
                        ie.EntrepreneurDriverLicense.ExprerienceFromYear
                    ),
                ie.INN,
                ie.OGRNIP,
                null,
                null,
                null,
                null,
                ie.IPBankAccount == null
                    ? null
                    : new BankInfoDto(
                        ie.IPBankAccount.CurrentAccount,
                        ie.IPBankAccount.CorrespondentAccount,
                        ie.IPBankAccount.BIK,
                        ie.IPBankAccount.BankName
                    ),
                ie.Documents
                    .Select(CustomerDocumentMapper.ToDto)
                    .ToList(),

                []
            ),
            
            _ => throw new NotSupportedException()
        };
    }

    public static Passport? ToDomain(CustomerPassportDto? dto)
    {
        if (dto is null)
            return null;

        return new Passport(
            dto.Serial,
            dto.Number,
            dto.IssuedBy,
            dto.IssuedDate,
            dto.RegistrationAddress
        );
    }

    public static DriverLicense? ToDomain(DriverLicenseDto? dto)
    {
        if (dto is null)
            return null;

        return new DriverLicense(
            dto.Number,
            dto.Categories,
            dto.IssuedDate,
            dto.ExpirationDate,
            dto.ExperienceFrom
        );
    }

    public static BankAccount? ToDomain(BankInfoDto? dto)
    {
        if (dto is null)
            return null;

        return new BankAccount(
            dto.CurrentAccount,
            dto.CorrespondentAccount,
            dto.Bik,
            dto.BankName
        );
    }
}