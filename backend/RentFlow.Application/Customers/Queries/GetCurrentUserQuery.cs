using MediatR;
using RentFlow.Application.Customers.Dtos;

namespace RentFlow.Application.Customers.Queries;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserDto>;