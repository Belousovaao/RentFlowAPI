using System;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public class BookingOrganizationSnapshot : BookingCustomerSnapshot
{
    public string FullName { get; }
    public string ShortName { get; }
    public string KPP { get; }
    public string OrganizationAdress { get; }
    public string FactAdress { get; }
    public BankAccount BankAccount { get; }

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
