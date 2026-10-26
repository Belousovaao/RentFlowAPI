using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Bookings.Queries;

namespace RentFlow.Api.Controllers;

[ApiController]
[Route("api/services")]
public class AdditionalServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdditionalServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result =
            await _mediator.Send(new GetAdditionalServicesQuery(), ct);

        return Ok(result);
    }
}