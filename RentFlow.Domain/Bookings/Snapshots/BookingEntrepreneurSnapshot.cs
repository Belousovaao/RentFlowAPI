using System;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public sealed class BookingEntrepreneurSnapshot : BookingCustomerSnapshot
{
    public PersonName Name { get; set; }
    public string OrganizationAdress { get; set; }
    public string FactAdress { get; set; }
    public BankAccount IPBankAccount { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
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
