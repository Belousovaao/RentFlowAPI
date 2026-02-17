using System;

namespace RentFlow.Domain.Customers;

public class IndividualEntrepreneur
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string INN { get; set; }
    public string OGRNIP { get; set; }
    public string CurrentAccount { get; set; }
    public string CorrespondentAccount { get; set; }
    public string BIK { get; set; }
    public string BankName { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public string FullName => $"Индивидуальный предприниматель {LastName} {FirstName} {MiddleName}".Trim();
    
    public string ShortName => $"ИП {LastName} {FirstName} {MiddleName}".Trim();
    public Guid RepresentativeId { get; set; }

}
