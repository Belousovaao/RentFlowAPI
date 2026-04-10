// Domain/Locations/Location.cs
using System;

namespace RentFlow.Domain.Locations;

public class Location
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Location() { }

    public Location(
        string name,
        string address,
        string city,
        string country,
        string postalCode,
        double? latitude,
        double? longitude,
        string phone,
        string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        City = city;
        Country = country;
        PostalCode = postalCode;
        Latitude = latitude;
        Longitude = longitude;
        Phone = phone;
        Email = email;
        IsActive = true;
    }
}