using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Domain.Customers;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Api.Services.Requests;
using RentFlow.Application.Customers.Commands;

namespace RentFlow.Api.Controllers;

[Route("api/admin/customers")]
[ApiController]
[Authorize(Roles = "admin")]
public class AdminCustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<CustomerShortDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool showRejected = false,
        [FromQuery] string? search = null,
        [FromQuery] CustomerType? type = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetAdminCustomersQuery(
                page,
                pageSize,
                search,
                type,
                showRejected),
            ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDetailsDto>> Get(Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetCustomerByIdQuery(id), ct));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDetailsDto>> Update(
        Guid id, 
        [FromBody] UpdateCustomerRequest request, 
        CancellationToken ct)
    {
        var customer = await _mediator.Send(new UpdateAdminCustomerCommand(id, request),
            ct);

        return Ok(customer);
    }

    [HttpPatch("{id:guid}/verificationStatus")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateVerificationStatusRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateCustomerVerificationStatusCommand(id,request.Status), ct);

        return NoContent();
    }

    [HttpGet("{customerId:guid}/documents")]
    public async Task<ActionResult<IReadOnlyList<CustomerDocumentDto>>> GetDocuments(Guid customerId, CancellationToken ct)
    {
        var documents = await _mediator.Send(new GetCustomerDocumentsQuery(customerId), ct);

        return Ok(documents);
    }

    [HttpDelete("{customerId:guid}/documents/{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(
        Guid customerId,
        Guid documentId,
        CancellationToken ct)
    {
        await _mediator.Send(
            new DeleteCustomerDocumentCommand(customerId, documentId),
            ct);

        return NoContent();
    }
}