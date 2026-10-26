using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Availability.Queries;

namespace RentFlow.Api.Controllers;

[ApiController]
[Route("api/availability")]
public sealed class AvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvailabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Занятые периоды автомобиля
    /// </summary>
    [HttpGet("assets/{assetId:guid}/busy-periods")]
    public async Task<IActionResult> GetBusyPeriods(
        Guid assetId,
        [FromQuery] Guid? excludeBookingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetBusyPeriodsQuery(assetId, excludeBookingId),
            ct);

        return Ok(result);
    }

    /// <summary>
    /// Автомобили, доступные на указанный период
    /// </summary>
    [HttpGet("assets")]
    public async Task<IActionResult> GetAvailableAssets(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetAvailableAssetsQuery(startDate, endDate),
            ct);

        return Ok(result);
    }
}