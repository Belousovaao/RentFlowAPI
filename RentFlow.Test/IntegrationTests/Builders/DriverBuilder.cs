using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Test.IntegrationTests.Builders;

public class DriverBuilder
{
    private string _firstName = "Ivan";
    private string _lastName = "Petrov";
    private string _middleName = "Ivanovich";
    private string _phone = "+7 (900) 000-22-00";
    private string _licenseNumber = "11 22 333333";
    private List<char> _categories = new() {'A', 'B'};
    private DateOnly _issued = new(2020, 1, 1);
    private DateOnly _expiration = new(2030, 1, 1);

    public DriverBuilder WithName(string firstName, string lastName, string middleName)
    {
        _firstName = firstName;
        _lastName = lastName;
        _middleName = middleName;
        return this;
    }

    public DriverBuilder WithLicense(string licenseNumber, List<char> categories, DateOnly issued, DateOnly expiration)
    {
        _licenseNumber = licenseNumber;
        _categories = categories;
        _issued = issued;
        _expiration = expiration;
        return this;
    }

    public DriverDto Build() => new(
        _firstName,
        _lastName,
        _middleName,
        _phone,
        _licenseNumber,
        _categories,
        _issued,
        _expiration
    );
}
