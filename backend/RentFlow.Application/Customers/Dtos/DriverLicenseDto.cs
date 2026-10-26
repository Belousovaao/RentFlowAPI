namespace RentFlow.Application.Customers.Dtos;

public sealed record DriverLicenseDto(
    string Number,
    List<string> Categories,
    DateOnly? IssuedDate,
    DateOnly? ExpirationDate,
    int? ExperienceFrom
);