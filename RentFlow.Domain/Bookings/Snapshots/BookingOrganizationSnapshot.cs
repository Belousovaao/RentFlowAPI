using System;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class BookingOrganizationSnapshot : BookingCustomerSnapshot
{
    public string FullName { get; set; }
    public string ShortName { get; set; }
    public string KPP { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public BankAccount BankAccount { get; set; }

    private BookingOrganizationSnapshot() {}
    public BookingOrganizationSnapshot(string fullName, string shortName, string kpp, string organizationAdress, string factAdress, BankAccount bankAccount)
    {
        FullName = fullName;
        ShortName = shortName;
        KPP = kpp;
        OrganizationAdress = organizationAdress;
        FactAdress = factAdress;
        BankAccount = bankAccount;
    }
}
