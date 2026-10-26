using MediatR;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;

public sealed class SearchCustomersHandler : IRequestHandler<SearchCustomersQuery, List<CustomerLookupDto>>
{
    private readonly ICustomerRepository _customers;

    public SearchCustomersHandler(ICustomerRepository customers)
    {
        _customers = customers;
    }

    public Task<List<CustomerLookupDto>> Handle(SearchCustomersQuery request, CancellationToken ct)
    {
        return _customers.SearchAsync(
                request.Type,
                request.Query,
                ct);
    }
}