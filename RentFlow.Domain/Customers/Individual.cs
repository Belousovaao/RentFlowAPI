using System;

namespace RentFlow.Domain.Customers;

public class Individual : Customer
{
    public PersonName Name { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public Passport? IndividualPassport { get; private set; }
    public DriverLicense? IndividualDriverLicense { get; private set; }
    public BankAccount? IndividualBankAccount { get; private set; }

    private Individual() {}
    public Individual(
        string? email, 
        string phone, 
        PersonName name, 
        DateOnly? dateOfBirth,
        Passport? passport,
        DriverLicense? driverLicense,
        BankAccount? bankAccount)
    {
        Id = Guid.NewGuid();
        Email = email;
        Phone = phone;
        Name = name;
        DateOfBirth = dateOfBirth;
        IndividualPassport = passport;
        IndividualDriverLicense = driverLicense;
        IndividualBankAccount = bankAccount;
    }

    public override Driver ResolveDriver(Driver? provideDriver)
    {
        return provideDriver ?? Driver.FromIndividual(this);
    }

}
