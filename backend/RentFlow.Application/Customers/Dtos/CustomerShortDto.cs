using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Dtos;
/// <summary>
/// Краткая информация о клиенте для отображения в ЛК клиента и админа
/// </summary>
public sealed record CustomerShortDto(
    Guid Id,
    CustomerType Type,
    string? DisplayName,
    string? Email,
    string? Phone,
    VerificationStatus VerificationStatus
); 