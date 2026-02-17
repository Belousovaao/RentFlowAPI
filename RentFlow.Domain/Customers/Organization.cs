using System;

namespace RentFlow.Domain.Customers;

public class Organization
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string ShortName { get; set; }
    public string INN { get; set; }
    public string OGRN { get; set; }
    public string KPP { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public Guid RepresentativeId { get; set; }
    public SigningBasis SigningBasis { get; set; }
    public string CurrentAccount { get; set; }
    public string CorrespondentAccount { get; set; }
    public string BIK { get; set; }
    public string BankName { get; set; }
}
