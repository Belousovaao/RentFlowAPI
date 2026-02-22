using System;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public sealed class BookingEntrepreneurSnapshot : BookingCustomerSnapshot
{
    public PersonName Name { get; }
    public string OrganizationAdress { get; }
    public string FactAdress { get; }
    public BankAccount IPBankAccount { get; }
    public string Email { get; }
    public string Phone { get; }
    private BookingEntrepreneurSnapshot() {}
    public BookingEntrepreneurSnapshot(PersonName name, string organizationadress, string factadress, BankAccount bankAccount, string phone, string email)
    {
        Name = name;
        OrganizationAdress = organizationadress;
        FactAdress = factadress;
        IPBankAccount = bankAccount;
        Phone = phone;
        Email = email;
    }
}
