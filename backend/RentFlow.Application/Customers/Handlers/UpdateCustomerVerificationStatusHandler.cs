using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Commands;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Mappers;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Handlers;

public sealed class UpdateCustomerVerificationStatusHandler : IRequestHandler<UpdateCustomerVerificationStatusCommand, CustomerDetailsDto>
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _uow;

    public UpdateCustomerVerificationStatusHandler(ICustomerRepository customers, IUnitOfWork uow)
    {
        _customers = customers;
        _uow = uow;
    }

    public async Task<CustomerDetailsDto> Handle(UpdateCustomerVerificationStatusCommand request, CancellationToken ct)
    {
        var customer = await _customers.GetByIdAsync(request.CustomerId, ct);

        if (customer is null)
            throw new NotFoundException("Customer not found", request.CustomerId);

        switch (request.Status)
        {
            case VerificationStatus.Pending:
                customer.SubmitForVerification();
                break;

            case VerificationStatus.Approved:
                customer.Approve();
                break;

            case VerificationStatus.Rejected:
                customer.Reject();
                break;

            case VerificationStatus.NotSubmitted:
                throw new InvalidOperationException("Cannot manually set status to NotSubmitted.");

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(request.Status),
                    request.Status,
                    null);
        }

        await _uow.SaveChangesAsync(ct);

        return CustomerMapper.ToDetailsDto(customer);
    }
}