using System;
using RentFlow.Domain.Bookings.Snapshots;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public sealed class BookingIndividualSnapshot : BookingCustomerSnapshot
{
    public PersonName Name { get; }
    public Passport Passport { get; }
    public string Email { get; }
    public string Phone { get; }
    private BookingIndividualSnapshot() {}
    public BookingIndividualSnapshot(PersonName name, Passport passport, string email, string phone)
    {
        Name = name;
        Passport = passport;
        Email = email;
        Phone = phone;
    }
}

