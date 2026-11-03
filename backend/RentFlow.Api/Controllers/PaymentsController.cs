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
    private readonly IPaymentAllocationRepository _allocations;

    public PaymentsController(IMediator mediator, IPaymentRepository payments, IPaymentAllocationRepository allocations)
    {
        _mediator = mediator;
        _payments = payments;
        _allocations = allocations;
    }

    [HttpGet("booking/{bookingId:guid}/transactions")]
    public async Task<IActionResult> GetTransactions(Guid bookingId, CancellationToken ct)
    {
        if (bookingId == Guid.Empty)
            return BadRequest("BookingId is required.");

        var payments = await _payments.GetByBookingIdAsync(bookingId, ct);
        var allocations = await _allocations.GetByBookingIdAsync(bookingId, ct);
        var refundsByPayment = payments
            .Where(p => p.Type == PaymentType.Refund && p.OriginalPaymentId.HasValue)
            .GroupBy(p => p.OriginalPaymentId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

        return Ok(payments.Select(p =>
        {
            var allocated = p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment
                ? allocations.Where(a => a.PaymentId == p.Id).Sum(a => a.Amount)
                : 0m;
            var refunded = refundsByPayment.GetValueOrDefault(p.Id);
            var availableToAllocate = p.Type is PaymentType.Prepayment or PaymentType.AdditionalPayment
                ? Math.Max(0m, p.Amount - allocated - refunded)
                : 0m;

            return new PaymentTransactionDto(
                p.Id,
                p.BookingId,
                p.OriginalPaymentId,
                p.Amount,
                p.Currency,
                p.Type,
                p.Method,
                p.ExternalOperationId,
                p.CreatedAt,
                p.Note,
                availableToAllocate
            );
        }));
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


}

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
    string? Note,
    decimal AvailableToAllocate);
