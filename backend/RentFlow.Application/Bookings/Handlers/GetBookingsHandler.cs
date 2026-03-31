using System;
using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Bookings.Queries;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Handlers;

public class GetBookingsHandler : IRequestHandler<GetBookingsQuery, List<BookingDto>>
{
    private readonly IBookingRepository _repo;

    public GetBookingsHandler(IBookingRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BookingDto>> Handle(GetBookingsQuery request, CancellationToken ct)
    {
        var bookings = await _repo.GetAllAsync();

        return (bookings.Select(b => new BookingDto(
            b.Id,
            b.AssetId,
            b.CustomerId,
            b.RentalPeriod,
            b.TotalPrice,
            b.Status
        )).ToList()); 
    }
}
