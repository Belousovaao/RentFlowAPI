using System;

namespace RentFlow.Domain.Customers;

public sealed class DriverLicense
{
    public string Number { get; set; }
    public List<char> Categories { get; private set; }
    public DateOnly? IssuedDate { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }

    private DriverLicense() { }

    public DriverLicense(
        string number,
        List<char> categories,
        DateOnly issuedDate,
        DateOnly expirationDate)
    {
        Number = number;
        Categories = categories;
        IssuedDate = issuedDate;
        ExpirationDate = expirationDate;
    }

    public bool IsExpired(DateOnly today) =>
        today > ExpirationDate;

    public string DriverLicenseFull => 
        $"ВУ №{Number} от {IssuedDate}";
}

