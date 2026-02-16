using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;

namespace RentFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repo;

        public BookingController(IBookingRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _repo.GetAllAsync();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            await _repo.AddAsync(booking);
            return Ok(booking);
        }
    }
}
