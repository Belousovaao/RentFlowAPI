using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Bookings.Commands;
using MediatR;
using RentFlow.Application.Bookings.Queries;

namespace RentFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _mediator.Send(new GetBookingsQuery());
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingCommand cmd, CancellationToken ct)
        {
            var booking = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(Create), booking);
        }
    }
}
