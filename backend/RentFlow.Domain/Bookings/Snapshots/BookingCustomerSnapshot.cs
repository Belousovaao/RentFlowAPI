using System;
using RentFlow.Domain.Common;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings.Snapshots;


public class BookingCustomerSnapshot
{
    public CustomerType Type { get; private set; }

    public PersonName? Name { get; private set; }
    public Passport? Passport { get; private set; }

    public string? Email { get; private set; }
    public string? Phone { get; private set; }

    public string? OrganizationName { get; private set; }
    public string? ShortName { get; private set; }
    public string? KPP { get; private set; }

    public string? OrganizationAddress { get; private set; }
    public string? FactAddress { get; private set; }

    public BankAccount? BankAccount { get; private set; }

    private BookingCustomerSnapshot() { }

    public static BookingCustomerSnapshot FromCustomer(Customer customer)
    {
        return customer switch
        {
            Individual i => new BookingCustomerSnapshot
            {
                Type = CustomerType.Individual,
                Name = i.Name,
                Passport = i.IndividualPassport,
                Email = i.Email,
                Phone = i.Phone
            },

            IndividualEntrepreneur e => new BookingCustomerSnapshot
            {
                Type = CustomerType.IndividualEntrepreneur,
                Name = e.Name,
                OrganizationAddress = e.OrganizationAdress,
                FactAddress = e.FactAdress,
                BankAccount = e.IPBankAccount,
                Phone = e.Phone,
                Email = e.Email
            },

            Organization o => new BookingCustomerSnapshot
            {
                Type = CustomerType.Organization,
                OrganizationName = o.FullName,
                ShortName = o.ShortName,
                KPP = o.KPP,
                OrganizationAddress = o.OrganizationAddress,
                FactAddress = o.FactAddress,
                BankAccount = o.OrganizationBankAccount
            },

            _ => throw new UnsupportedCustomerType()
        };
    }
}