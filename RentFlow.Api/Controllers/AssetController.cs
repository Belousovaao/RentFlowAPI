using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Assets.Queries;

namespace RentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/assets")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetRepository _repo;

        public AssetsController(IAssetRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AssetFilter filter, CancellationToken ct)
        {
            var result = await _repo.SearchAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
        {
            var asset = await _repo.GetByCodeAsync(code, ct);
            if (asset == null)
                return NotFound();

            return Ok(asset);
        }
    }
}
