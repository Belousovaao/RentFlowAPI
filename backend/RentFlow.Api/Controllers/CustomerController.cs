using Microsoft.AspNetCore.Mvc;
using RentFlow.Domain.Customers;
using RentFlow.Application.Customers.Queries;
using MediatR;
using RentFlow.Application.Customers.Dtos;
using Microsoft.AspNetCore.Authorization;
using RentFlow.Application.Customers.Commands;
using System.Security.Claims;

namespace RentFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CustomerDetailsDto>> GetMe(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var customer = await _mediator.Send(new GetCurrentCustomerQuery(userId), ct);

            return Ok(customer);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<CustomerDetailsDto>> Update(
            [FromBody] UpdateCustomerRequest request,
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var customer = await _mediator.Send(new UpdateCustomerCommand(userId, request), ct);
            return Ok(customer);
        }

    }
}
