using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Payments.Commands;
using RentFlow.Application.Payments.Queries;
using RentFlow.Domain.Payments;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPaymentRepository _payments;

    public PaymentsController(IMediator mediator, IPaymentRepository payments)
    {
        _mediator = mediator;
        _payments = payments;
    }

    [HttpGet("booking/{bookingId:guid}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid bookingId, CancellationToken ct)
    {
        if (bookingId == Guid.Empty)
            return BadRequest("BookingId is required.");

        var payments = await _payments.GetByBookingIdAsync(bookingId, ct);
        return Ok(payments.Select(p => new PaymentTransactionDto(
            p.Id,
            p.BookingId,
            p.OriginalPaymentId,
            p.Amount,
            p.Currency,
            p.Type,
            p.Method,
            p.ExternalOperationId,
            p.CreatedAt,
            p.Note
        )));
    }

    [HttpGet("booking/{bookingId:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid bookingId, CancellationToken ct)
    {
        var summary = await _mediator.Send(new GetBookingPaymentSummaryQuery(bookingId), ct);
        return Ok(summary);
    }

    [HttpPost("incoming")]
    public async Task<IActionResult> RegisterIncoming(
        [FromBody] RegisterIncomingPaymentCommand command,
        CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return Ok(new { id });
    }

    [HttpPost("refunds")]
    public async Task<IActionResult> CreateRefund(
        [FromBody] CreateRefundCommand command,
        CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return Created($"/api/payments/{id}", new { id });
    }

    [HttpPost("refunds/{refundPaymentId:guid}/complete")]
    public async Task<IActionResult> CompleteRefund(
        Guid refundPaymentId,
        [FromBody] CompleteRefundRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new CompleteRefundCommand(refundPaymentId, request.ExternalOperationId), ct);
        return NoContent();
    }
}

public sealed record CompleteIncomingPaymentRequest(string? ExternalOperationId);
public sealed record CompleteRefundRequest(string? ExternalOperationId);

public sealed record PaymentTransactionDto(
    Guid Id,
    Guid BookingId,
    Guid? OriginalPaymentId,
    decimal Amount,
    string Currency,
    PaymentType Type,
    PaymentMethod Method,
    string? ExternalOperationId,
    DateTimeOffset CreatedAt,
    string? Note);
