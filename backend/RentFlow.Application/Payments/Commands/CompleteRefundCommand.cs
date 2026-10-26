using MediatR;

namespace RentFlow.Application.Payments.Commands;

public sealed record CompleteRefundCommand(
    Guid RefundPaymentId,
    string? ExternalOperationId = null
) : IRequest;