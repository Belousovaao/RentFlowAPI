using System;
using RentFlow.Domain.Customers;
using RentFlow.Domain.Customers.ValueObjects;

namespace RentFlow.Test.IntegrationTests.Builders;

public class CustomerBuilder
{
    private Func<Customer> _factory = DefaultIndividual;

    private static Customer DefaultIndividual()
        => Customer.CreateIndividual(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            dateOfBirth: new DateOnly(1992, 12, 2),
            passport: new Passport("1111", "222222", "Otdelom UFMS Rossii", new DateOnly(2000,1,1), "Moscow, Tsvetochanaya st., 555 apt."),
            driverLicense: new DriverLicense("11 22 333333", new List<char>{'A', 'B'}, new DateOnly(2001, 2, 2), new DateOnly(2030, 12, 31)), 
            bankAccount: new BankAccount("11111111111111111", "222222222222222222222222", "09454950", "VTB-Bank"));
    public CustomerBuilder AsEntrepreneur()
    {
         _factory = () => Customer.CreateIndividualEntrepreneur(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            name: new PersonName("Vasya", "Pupkin", "Nikitich"),
            passport: TestData.ValidPassport(),
            inn: "34354656",
            ogrnip: "4678909876543556",
            organizationAddress: "Russia, Moscow, Cheburechnaya st. 9",
            factAdress: "Russia, Moscow, Cheburechnaya st. 9",
            bankAccount: TestData.ValidBankAccount(),
            representativeId: null,
            driverLicense: TestData.ValidDriverLicense());

            return this;
    }

    public CustomerBuilder AsOrganization()
    {
         _factory = () => Customer.CreateOrganization(
            email: "vasyapupkin@mail.ru",
            phone: "+7 (800) 0000-00-00",
            fullName: "Строительная компания Ромашкина",
            shortName: "СКР",
            legalForm: OrganizationLegalForm.OOO,
            inn: "3545563556",
            ogrn: "45678909876567589856",
            kpp: "24324243",
            organizationAddress: "Russia, Moscow, Oduvanchikov st. 9",
            factAddress: "Russia, Moscow, Oduvanchikov st. 9",
            representativeId: Guid.NewGuid(),
            signingBasis: SigningBasis.ByCharter(),
            bankAccount: TestData.ValidBankAccount());

            return this;
    }

    public Customer Build() => _factory();
}
