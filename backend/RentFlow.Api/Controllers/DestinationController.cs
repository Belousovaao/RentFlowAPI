using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DestinationController : ControllerBase
{
    private readonly IMediator _mediator;

    public DestinationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive(
        [FromQuery] DestinationType type,
        CancellationToken ct)
    {
        var destinations = await _mediator.Send(
            new GetActiveDestinationsQuery(type),
            ct);

        return Ok(destinations);
    }
}