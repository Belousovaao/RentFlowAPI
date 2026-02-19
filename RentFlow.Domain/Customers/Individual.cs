using System;

namespace RentFlow.Domain.Customers;

public class Individual : Customer
{
    public PersonName Name { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Passport? IndividualPassport { get; set; }
    public DriverLicense? IndividualDriverLicense { get; set; }
    public BankAccount IndividualBankAccount { get; set; }
}
