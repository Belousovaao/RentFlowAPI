using System;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Domain.Customers;

public class Organization : Customer
{
    public string FullName { get; private set; }
    public string ShortName { get; private set; }
    public OrganizationLegalForm LegalForm { get; private set; }
    public string INN { get; private set; }
    public string OGRN { get; private set; }
    public string KPP { get; private set; }
    public string OrganizationAddress { get; private set; }
    public string FactAddress { get; private set; }
    public Guid RepresentativeId { get; private set; }
    public SigningBasis SigningBasis { get; private set; }
    public BankAccount OrganizationBankAccount { get; private set; }

    private Organization() {}
    public Organization(
        string? email,
        string phone,
        string fullName,
        string shortName,
        OrganizationLegalForm legalForm,
        string inn,
        string ogrn,
        string kpp,
        string organizationAddress,
        string factAddress,
        Guid representativeId,
        SigningBasis signingBasis,
        BankAccount bankAccount
        )
    {
        Id = Guid.NewGuid();
        Email = email;
        Phone = phone;
        FullName = fullName;
        ShortName = shortName;
        LegalForm = legalForm;
        INN = inn;
        OGRN = ogrn;
        KPP = kpp;
        OrganizationAddress = organizationAddress;
        FactAddress = factAddress;
        RepresentativeId = representativeId;
        SigningBasis = signingBasis;
        OrganizationBankAccount = bankAccount;
    }

    public override Driver ResolveDriver(Driver? provideDriver)
    {
        if (provideDriver is null)
            throw new ArgumentException("Driver is required for organization");

        return provideDriver;
    }
}