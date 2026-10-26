using System;

namespace RentFlow.Domain.Customers;

public sealed class DriverLicense
{
    public string Number { get; set; }
    public List<string> Categories { get; private set; }
    public DateOnly? IssuedDate { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }
    public int? ExprerienceFromYear { get; private set; }

    private DriverLicense() { }

    public DriverLicense(
        string number,
        List<string> categories,
        DateOnly? issuedDate,
        DateOnly? expirationDate,
        int? experienceFromYear)
    {
        Number = number;
        Categories = categories;
        IssuedDate = issuedDate;
        ExpirationDate = expirationDate;
        ExprerienceFromYear = experienceFromYear;
    }

    public bool IsExpired(DateOnly today) =>
        today > ExpirationDate;

    public string DriverLicenseFull => 
        $"ВУ №{Number} от {IssuedDate}";
}

