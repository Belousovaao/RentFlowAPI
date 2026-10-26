using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Customers.Queries;
public record GetAdminCustomersQuery(
    int Page,
    int PageSize,
    string? Search,
    CustomerType? Type,
    bool ShowRejected
) : IRequest<PaginatedResponse<CustomerShortDto>>;