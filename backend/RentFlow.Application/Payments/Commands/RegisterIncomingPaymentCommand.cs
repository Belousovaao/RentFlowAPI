
using MediatR;
using RentFlow.Domain.Payments;

namespace RentFlow.Application.Payments.Commands;

public sealed record RegisterIncomingPaymentCommand(
    Guid BookingId,
    decimal Amount,
    PaymentType Type,
    PaymentMethod Method,
    string? Note = null
) : IRequest<Guid>;
