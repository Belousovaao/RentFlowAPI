using System;

namespace RentFlow.Domain.Customers;

public class Individual
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PassportSerial { get; set; }
    public string? PassportNumber { get; set; }
    public string? PassportWhomGiven { get; set; }
    public DateOnly? PassportWhenGiven { get; set; }
    public string? RegistrationAddress { get; set; }
    public char? DriverLicenseCategory { get; set; }
    public DateOnly? DriverLicenseGivenDate { get; set; }
    public DateOnly? DriverLicensExpirationDate { get; set; }

}
