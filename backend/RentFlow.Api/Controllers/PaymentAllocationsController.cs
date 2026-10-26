using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.BookingCharges.Commands;
namespace RentFlow.Api.Controllers;
[ApiController]
[Authorize(Roles = "admin")]
[Route("api/payments")]
public sealed class PaymentAllocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentAllocationsController(IMediator mediator) => _mediator = mediator;
    [HttpPut("{paymentId:guid}/allocations")]
    public async Task<IActionResult> Allocate(Guid paymentId, [FromBody] AllocatePaymentRequest request, CancellationToken ct)
    { await _mediator.Send(new AllocatePaymentToChargesCommand(paymentId, request.Allocations), ct); return NoContent(); }
}
public sealed record AllocatePaymentRequest(IReadOnlyList<PaymentAllocationItem> Allocations);