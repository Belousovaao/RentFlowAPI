using MediatR;
using RentFlow.Application.Bookings.Dtos;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Bookings.Queries;

public sealed class GetCustomerBookingDetailsHandler : IRequestHandler<GetCustomerBookingDetailsQuery, BookingDetailsDto?>
{
    private readonly IBookingReadRepository _bookingReadRepository;

    public GetCustomerBookingDetailsHandler(IBookingReadRepository bookingReadRepository)
    {
        _bookingReadRepository = bookingReadRepository;
    }

    public async Task<BookingDetailsDto?> Handle(GetCustomerBookingDetailsQuery request, CancellationToken ct)
    {
        return await _bookingReadRepository.GetCustomerBookingDetailsAsync(
            request.CustomerId,
            request.BookingId,
            ct);
    }
}