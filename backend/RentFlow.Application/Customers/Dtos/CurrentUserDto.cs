using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Dtos;
public sealed record CurrentUserDto
(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string Role,
    Guid? CustomerId
);