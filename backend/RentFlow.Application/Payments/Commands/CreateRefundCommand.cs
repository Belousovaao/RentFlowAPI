using MediatR;

namespace RentFlow.Application.Payments.Commands;

public sealed record CreateRefundCommand(
    Guid OriginalPaymentId,
    decimal Amount,
    string? Note = null
) : IRequest<Guid>;