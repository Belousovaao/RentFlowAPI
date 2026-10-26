using MediatR;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Customers.Handlers;
public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IAppUserRepository _users;
    public GetCurrentUserHandler(IAppUserRepository users)
    {
        _users = users;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user is null)
            throw new NotFoundException("User not found", request.UserId);

        return new CurrentUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Role,
            user.CustomerId
        );
    }
}