using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.RentalTariffs;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/rental-tariffs")]
public sealed class RentalTariffsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RentalTariffsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetRentalTariffsQuery(), ct));

    [HttpPut("{code}")]
    public async Task<IActionResult> Upsert(
        string code,
        [FromBody] UpdateRentalTariffRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpsertRentalTariffCommand(code, request.Price, request.Unit), ct);
        return Ok(result);
    }
}

public sealed record UpdateRentalTariffRequest(decimal Price, string Unit);
