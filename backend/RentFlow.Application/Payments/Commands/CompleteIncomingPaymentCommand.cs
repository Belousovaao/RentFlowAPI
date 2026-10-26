using MediatR;

namespace RentFlow.Application.Payments.Commands;

public sealed record CompleteIncomingPaymentCommand(
    Guid PaymentId,
    string? ExternalOperationId = null
) : IRequest;