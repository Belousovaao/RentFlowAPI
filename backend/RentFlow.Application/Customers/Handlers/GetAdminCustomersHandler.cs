
using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Handlers;
public class GetCustomersHandler : IRequestHandler<GetAdminCustomersQuery, PaginatedResponse<CustomerShortDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PaginatedResponse<CustomerShortDto>> Handle(GetAdminCustomersQuery request, CancellationToken ct)
    {
        var result = await _customerRepository.GetListAsync(
            request.Page,
            request.PageSize,
            request.ShowRejected,
            request.Search,
            request.Type,
            ct);

        var totalPages =(int)Math.Ceiling(result.TotalCount / (double)request.PageSize);

        return new PaginatedResponse<CustomerShortDto>(
            result.Data,
            result.TotalCount,
            request.Page,
            request.PageSize,
            totalPages,
            request.Page < totalPages,
            request.Page > 1
        );
    }
}