namespace RentFlow.Application.Customers.Dtos;

public sealed record BankInfoDto(
    string? CurrentAccount,
    string? CorrespondentAccount,
    string? Bik,
    string? BankName
);