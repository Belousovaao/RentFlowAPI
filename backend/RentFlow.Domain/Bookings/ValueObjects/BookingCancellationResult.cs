namespace RentFlow.Domain.Bookings.ValueObjects;

public sealed record BookingCancellationResult(
    decimal PrepaymentReceived,
    decimal RetainedAmount,
    decimal RefundAmount);