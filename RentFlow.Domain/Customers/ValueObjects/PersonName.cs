using System;

namespace RentFlow.Domain.Customers;

public record PersonName
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? MiddleName { get; private set; }

    private PersonName() { }

    public PersonName(string firstName, string lastName, string? middleName)
    {
        FirstName = firstName.Trim() ?? throw new ArgumentException("First name required");
        LastName = lastName.Trim() ?? throw new ArgumentException("Last name required");
        MiddleName = string.IsNullOrWhiteSpace(middleName)
            ? null
            : middleName.Trim();
    }

    public string? FullName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return null;

            if (string.IsNullOrWhiteSpace(MiddleName))
                return $"{LastName} {FirstName}";
            
            return $"{LastName} {FirstName} {MiddleName}";
        }
    }

    public string? ShortName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return null;

            if (string.IsNullOrWhiteSpace(MiddleName))
                return $"{LastName} {FirstName[0]}.";
            
            return $"{LastName} {FirstName[0]}.{MiddleName[0]}.";
        }
    }
}

