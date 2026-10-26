
namespace RentFlow.Application.Payments.Dtos;

public sealed record BookingPaymentSummaryDto(
    Guid BookingId,
    string Currency,
    decimal TotalReceived,
    decimal TotalRefunded,
    decimal PendingRefunds,
    decimal NetPaid,
    decimal AvailableToRefund
);
