using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Handlers;
public sealed class GetAdminBookingsHandler: IRequestHandler<GetAdminBookingsQuery, IReadOnlyList<AdminBookingListItemDto>>
{
    private readonly IBookingReadRepository _repo;

    public GetAdminBookingsHandler(IBookingReadRepository repo)
    {
        _repo = repo;
    }
    public async Task<IReadOnlyList<AdminBookingListItemDto>> Handle(GetAdminBookingsQuery request, CancellationToken ct)
    {
        return await _repo.GetAdminBookingsAsync(ct);
    }
}