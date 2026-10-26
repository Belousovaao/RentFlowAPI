using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Api.Services.Requests;
public sealed record PreviewBookingServicesRequest(
    List<BookingServiceRequestDto> Services);