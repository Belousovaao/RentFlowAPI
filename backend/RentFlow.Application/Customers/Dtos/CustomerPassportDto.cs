namespace RentFlow.Application.Customers.Dtos;

public sealed record CustomerPassportDto(
    string Serial,
    string Number,
    string IssuedBy,
    DateOnly? IssuedDate,
    string RegistrationAddress
);