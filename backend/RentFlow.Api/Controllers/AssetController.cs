using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Assets.Queries;
using MediatR;

namespace RentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssetsController (IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AssetFilter filter, CancellationToken ct)
        {
            var result = await _mediator.Send(new SearchAssetsQuery(filter), ct);
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var asset = await _mediator.Send(new GetAssetByCodeQuery(code), ct);
            if (asset == null)
                return NotFound();

            return Ok(asset);
        }
    }
}
