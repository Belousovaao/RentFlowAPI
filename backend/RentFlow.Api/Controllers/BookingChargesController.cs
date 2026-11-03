using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Services.Auth;
using RentFlow.Application.BookingCharges.Commands;
using RentFlow.Application.BookingCharges.Queries;
namespace RentFlow.Api.Controllers;
[ApiController]
[Route("api/booking-charges")]
public sealed class BookingChargesController : ControllerBase
{
    private readonly IMediator _mediator;
    public BookingChargesController(IMediator mediator) => _mediator = mediator;
    [Authorize]
    [HttpGet("booking/{bookingId:guid}")]
    public async Task<IActionResult> GetForBooking(Guid bookingId, CancellationToken ct)
    {
        Guid? customerId = User.IsInRole("admin") ? null : User.GetCustomerId();
        return Ok(await _mediator.Send(new GetBookingChargesQuery(bookingId, customerId), ct));
    }
    [Authorize(Roles = "admin")]
    [HttpPost("admin/booking/{bookingId:guid}")]
    public async Task<IActionResult> Create(Guid bookingId, [FromBody] CreateBookingChargeCommand command, CancellationToken ct)
    {
        if (bookingId != command.BookingId) return BadRequest("BookingId in route and body must match.");
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetForBooking), new { bookingId }, new { id });
    }
    [Authorize(Roles = "admin")]
    [HttpPost("admin/booking/{bookingId:guid}/{chargeId:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid bookingId, Guid chargeId, CancellationToken ct)
    { await _mediator.Send(new ReviewBookingChargeCommand(bookingId,chargeId,true), ct); return NoContent(); }
    [Authorize(Roles = "admin")]
    [HttpPost("admin/booking/{bookingId:guid}/{chargeId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid bookingId, Guid chargeId, CancellationToken ct)
    { await _mediator.Send(new ReviewBookingChargeCommand(bookingId,chargeId,false), ct); return NoContent(); }
}