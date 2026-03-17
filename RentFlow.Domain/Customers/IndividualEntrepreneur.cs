using System;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Domain.Customers;

public class IndividualEntrepreneur : Customer
{
    public PersonName Name { get; private set; }
    public Passport IPPassport { get; private set; }
    public string INN { get; private set; }
    public string OGRNIP { get; private set; }
    public string OrganizationAdress { get; private set; }
    public string FactAdress { get; private set; }
    public BankAccount IPBankAccount { get; private set; }
    public Guid? RepresentativeId { get; private set; }
    public OrganizationLegalForm IpLegalForm { get; private set; }
    public DriverLicense? EntrepreneurDriverLicense { get; private set; }


    private IndividualEntrepreneur() {}
    public IndividualEntrepreneur(
        string? email,
        string phone,
        PersonName name,
        Passport passport,
        string inn,
        string ogrnip,
        string organizationAddress,
        string factAdress,
        BankAccount bankAccount,
        Guid? representativeId,
        DriverLicense? driverLicense)
    {
        Id = Guid.NewGuid();
        IpLegalForm = OrganizationLegalForm.IP;
        Email = email;
        Phone = phone;
        Name = name;
        IPPassport = passport;
        INN = inn;
        OGRNIP = ogrnip;
        OrganizationAdress = organizationAddress;
        FactAdress = factAdress;
        IPBankAccount = bankAccount;
        RepresentativeId = representativeId;
        EntrepreneurDriverLicense = driverLicense;
    }

    public override Driver ResolveDriver(Driver? provideDriver)
    {
        return provideDriver ?? Driver.Create(Name, EntrepreneurDriverLicense, Phone);
    }

}
