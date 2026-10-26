using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Mappers;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;

public sealed class GetCurrentCustomerHandler : IRequestHandler<GetCurrentCustomerQuery, CustomerDetailsDto>
{
    private readonly IAppUserRepository _users;
    private readonly ICustomerRepository _customers;


    public GetCurrentCustomerHandler(IAppUserRepository users, ICustomerRepository customers)
    {
        _users = users;
        _customers = customers;
    }


    public async Task<CustomerDetailsDto> Handle(GetCurrentCustomerQuery request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        
        if (user is null)
            throw new NotFoundException("User not found", request.UserId);

        if (user.CustomerId is null)
            throw new NotFoundException("Customer profile not found", request.UserId);

        var customer = await _customers.GetByIdAsync(user.CustomerId.Value, ct);

        if (customer is null)
            throw new NotFoundException("Customer not found", user.CustomerId.Value);

        return CustomerMapper.ToDetailsDto(customer);
    }
}