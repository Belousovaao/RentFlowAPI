using System;

namespace RentFlow.Domain.Customers;

public class Organization : Customer
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string ShortName { get; set; }
    public OrganizationForm ShortOrganizationForm { get; set; }
    public OrganizationForm FullOrganizationForm { get; set; }
    public string INN { get; set; }
    public string OGRN { get; set; }
    public string KPP { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public Guid RepresentativeId { get; set; }
    public SigningBasis SigningBasis { get; set; }
    public BankAccount OrganizationBankAccount { get; set; }
}

public enum OrganizationForm
{
    OOO,
    AO,
    KFH,

}
