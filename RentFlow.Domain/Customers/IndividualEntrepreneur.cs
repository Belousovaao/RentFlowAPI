using System;

namespace RentFlow.Domain.Customers;

public class IndividualEntrepreneur : Customer
{
    public PersonName Name { get; set; }
    public Passport IPPassport { get; set; }
    public string INN { get; set; }
    public string OGRNIP { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public BankAccount IPBankAccount { get; set; }
    public Guid RepresentativeId { get; set; }

}
