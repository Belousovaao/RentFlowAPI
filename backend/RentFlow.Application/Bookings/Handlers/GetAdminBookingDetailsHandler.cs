using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Customers;

namespace RentFlow.Application.Bookings.Handlers;

public sealed class GetAdminBookingDetailsHandler: IRequestHandler<GetAdminBookingDetailsQuery, AdminBookingDetailsDto>
{
    private readonly IBookingReadRepository _repo;

    public GetAdminBookingDetailsHandler(IBookingReadRepository repo)
    {
        _repo = repo;
    }

    public async Task<AdminBookingDetailsDto> Handle(GetAdminBookingDetailsQuery request, CancellationToken ct)
    {
        var result = await _repo.GetAdminBookingDetailsAsync(request.BookingId, ct);
        if (result is null)
            throw new InvalidOperationException("Booking not found.");
        return result;
    }
}