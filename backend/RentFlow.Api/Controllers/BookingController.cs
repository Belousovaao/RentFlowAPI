using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Bookings.Commands;
using MediatR;
using RentFlow.Application.Bookings.Queries;
using Microsoft.AspNetCore.Authorization;
using RentFlow.Application.Availability.Queries;
using RentFlow.Api.Services.Auth;
using RentFlow.Api.Services.Requests;
using RentFlow.Application.Customers.Commands;

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
        // ---------------------------------------------------------
        // CUSTOMER
        // ---------------------------------------------------------

        [Authorize(Roles = "user")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings(CancellationToken ct)
        {
            var customerId = User.GetCustomerId();

            var bookings = await _mediator.Send(new GetCustomerBookingsQuery(customerId), ct);

            return Ok(bookings);
        }

        [Authorize]
        [HttpGet("my/{id:guid}")]
        public async Task<IActionResult> GetMyBooking(
            Guid id,
            CancellationToken ct)
        {
            var customerId = User.GetCustomerId();

            var booking = await _mediator.Send(
                new GetCustomerBookingDetailsQuery(
                    customerId,
                    id),
                ct);

            if (booking is null)
                return NotFound();

            return Ok(booking);
        }

        // ---------------------------------------------------------
        // ADMIN
        // ---------------------------------------------------------
        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllForAdmin(CancellationToken ct)
        {
            var bookings = await _mediator.Send(new GetAdminBookingsQuery(), ct);
            return Ok(bookings);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/{id:guid}")]
        public async Task<IActionResult> GetForAdmin(Guid id, CancellationToken ct)
        {
            var booking = await _mediator.Send(new GetAdminBookingDetailsQuery(id), ct);

            if (booking is null)
                return NotFound();

            return Ok(booking);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("admin/{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeBookingStatusCommand command, CancellationToken ct)
        {
            if (id != command.BookingId)
                return BadRequest();

            await _mediator.Send(command, ct);

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> CreateByAdmin([FromBody] CreateBookingCommand cmd, CancellationToken ct)
        {
            var booking = await _mediator.Send(cmd, ct);

            return Ok(booking);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/customer/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerBookings(Guid customerId, CancellationToken ct)
        {
            var bookings = await _mediator.Send(new GetCustomerBookingsQuery(customerId), ct);

            return Ok(bookings);
        }

        // ---------------------------------------------------------
        // CREATE BOOKING
        // ---------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingCommand cmd, CancellationToken ct)
        {
            var booking = await _mediator.Send(cmd, ct);
            return Ok(booking);
        }

        // ---------------------------------------------------------
        // CUSTOMER DOCUMENTS
        // ---------------------------------------------------------
        [Authorize]
        [HttpPost("documents")]
        public async Task<IActionResult> UploadDocuments([FromForm] UploadCustomerDocumentsRequest request, CancellationToken ct)
        {
            var customerId = User.GetCustomerId();

            var documentId = await _mediator.Send(new UploadCustomerDocumentCommand(
                customerId,
                request.Type,
                request.File.OpenReadStream(),
                request.File.FileName
            ), ct);

            return Ok(documentId);
        }

        [HttpPost("calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculateBookingPriceQuery query, CancellationToken ct)
        {
            var price = await _mediator.Send(query, ct);

            return Ok(price);
        }

        [HttpPatch("{id:guid}/period")]
        public async Task<IActionResult> ChangePeriod(Guid id, [FromBody] ChangeBookingPeriodRequest request, CancellationToken ct)
        {
            var command = new ChangeBookingPeriodCommand(
                id,
                request.StartDate,
                request.EndDate);

            await _mediator.Send(command, ct);

            return NoContent();
        }

        [Authorize]
        [HttpPost("{id:guid}/period/preview")]
        public async Task<IActionResult> PreviewChangePeriod(Guid id, [FromBody] ChangeBookingPeriodRequest request, CancellationToken ct)
        {
            Guid? customerId = null;

            if (!User.IsInRole("admin"))
            {
                customerId = User.GetCustomerId();
            }

            var query = new PreviewChangeBookingPeriodQuery(
                id,
                request.StartDate,
                request.EndDate,
                customerId);

            var preview = await _mediator.Send(query, ct);

            return Ok(preview);
        }

        [Authorize]
        [HttpPost("{id:guid}/services/preview")]
        public async Task<IActionResult> PreviewChangeServices(
            Guid id,
            [FromBody]PreviewBookingServicesRequest request,
            CancellationToken ct)
        {
            Guid? customerId = null;

            if (!User.IsInRole("admin"))
            {
                customerId = User.GetCustomerId();
            }

            var preview = await _mediator.Send(
                new PreviewBookingServicesChangeQuery(
                    id,
                    request.Services,
                    customerId),
                ct);

            return Ok(preview);
        }

        [Authorize]
        [HttpPatch("{id:guid}/services")]
        public async Task<IActionResult> UpdateServices(
            Guid id,
            [FromBody] UpdateBookingServicesRequest request,
            CancellationToken ct)
        {
            Guid? customerId = null;

            if (!User.IsInRole("admin"))
            {
                customerId = User.GetCustomerId();
            }
            await _mediator.Send(
                new UpdateBookingServicesCommand(
                    id,
                    request.Services,
                    customerId),
                ct);

            return NoContent();
        }

        [Authorize]
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(
            Guid id,
            [FromBody] CancelBookingRequest request,
            CancellationToken ct)
        {
            Guid? customerId = null;

            if (!User.IsInRole("admin"))
            {
                customerId = User.GetCustomerId();
            }

            await _mediator.Send(
                new CancelBookingCommand(id, request.Reason, customerId),
                ct);

            return NoContent();
        }  
    }
}
