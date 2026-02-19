using System;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Bookings;

public sealed class BookingIndividualSnapshot
{
    public PersonName Name { get; set; }
    public Passport Passport { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    private BookingIndividualSnapshot() {}
    public BookingIndividualSnapshot(PersonName name, Passport passport, string email, string phone)
    {
        Name = name;
        Passport = passport;
        Email = email;
        Phone = phone;
    }
}

