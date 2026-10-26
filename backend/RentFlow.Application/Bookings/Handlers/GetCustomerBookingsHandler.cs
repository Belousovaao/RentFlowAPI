using System;
using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class GetCustomerBookingsHandler : IRequestHandler<GetCustomerBookingsQuery, IReadOnlyList<BookingListItemDto>>
{
    private readonly IBookingReadRepository _repo;

    public GetCustomerBookingsHandler(IBookingReadRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<BookingListItemDto>> Handle(GetCustomerBookingsQuery request, CancellationToken ct)
    {
        return await _repo.GetCustomerBookingsAsync(request.CustomerId, ct);
    }
}
