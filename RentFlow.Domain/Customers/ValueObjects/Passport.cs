using System;
using System.Net.Http.Headers;

namespace RentFlow.Domain.Customers;

public sealed class Passport
{
    public string Serial { get; }
    public string Number { get; }
    public string? IssuedBy { get; }
    public DateOnly? IssuedDate { get; }
    public string? RegistrationAddress { get; set; }

    private Passport() { }

    public Passport(
        string serial,
        string number,
        string? issuedBy,
        DateOnly? issuedDate,
        string? registrationAddress)
    {
        if (string.IsNullOrWhiteSpace(serial))
            throw new ArgumentException("Serial required");

        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Number required");

        Serial = serial;
        Number = number;
        IssuedBy = issuedBy;
        IssuedDate = issuedDate;
        RegistrationAddress = registrationAddress;
    }

    public string FullPassport =>
        $"Паспорт РФ серия {Serial} №{Number}, выдан {IssuedBy} от {IssuedDate}, адрес регистрации: {RegistrationAddress}.";
}

