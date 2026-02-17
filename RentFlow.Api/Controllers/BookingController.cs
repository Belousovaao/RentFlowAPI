using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Bookings.Handlers;

namespace RentFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repo;
        private readonly CreateBookingHandler _handler;

        public BookingController(IBookingRepository repo, CreateBookingHandler handler)
        {
            _repo = repo;
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _repo.GetAllAsync();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingCommand cmd, CancellationToken ct)
        {
            Booking booking = await _handler.Handle(cmd, ct);
            return Ok(booking);
        }
    }
}
