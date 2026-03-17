using System;
using System.ComponentModel.DataAnnotations;
using RentFlow.Domain.Customers;

namespace RentFlow.Domain.Customers;

public class Driver
{
    public Guid Id { get; private set; }
    public PersonName Name { get; private set; }
    public DriverLicense License { get; private set; }
    public string Phone { get; private set; }

    private Driver() {}
 
    private Driver(PersonName name, DriverLicense license, string phone)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        License = license ?? throw new ArgumentNullException(nameof(license));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
    }

    public static Driver Create(PersonName name, DriverLicense license, string phone) => new(name, license, phone);
    public static Driver FromIndividual(Individual individual) => new(individual.Name, individual.IndividualDriverLicense, individual.Phone);
    public static Driver FromEntrepreneur(IndividualEntrepreneur entrepreneur) => new(entrepreneur.Name, entrepreneur.EntrepreneurDriverLicense, entrepreneur.Phone);
}
