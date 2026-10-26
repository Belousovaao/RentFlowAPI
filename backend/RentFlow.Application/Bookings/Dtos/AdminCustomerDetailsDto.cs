using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Dtos;
/// <summary>
/// Полная информация о клиенте
/// </summary>
/// <param name="Id"></param>
/// <param name="Type"></param>
/// <param name="DisplayName"></param>
/// <param name="Email"></param>
/// <param name="Phone"></param>
/// <param name="VerificationStatus"></param>
/// <param name="Documents"></param>
/// <param name="Bookings"></param>
public record AdminCustomerDetailsDto(
    Guid Id,
    CustomerType Type,
    string? DisplayName,
    string? Email,
    string Phone,
    VerificationStatus VerificationStatus,
    IReadOnlyCollection<CustomerDocumentDto> Documents,
    IReadOnlyCollection<BookingListItemDto> Bookings
);