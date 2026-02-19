using System;

namespace RentFlow.Domain.Customers;

public sealed class PersonName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }

    private PersonName() { }

    public PersonName(string firstName, string lastName, string? middleName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name required");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = string.IsNullOrWhiteSpace(middleName)
            ? null
            : middleName.Trim();
    }

    public string FullName =>
        MiddleName is null
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {MiddleName}";

    public string ShortName =>
        MiddleName is null
            ? $"{LastName} {FirstName[0]}."
            : $"{LastName} {FirstName[0]}.{MiddleName[0]}.";
}

