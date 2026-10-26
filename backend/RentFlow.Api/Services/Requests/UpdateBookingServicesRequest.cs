using RentFlow.Application.Bookings.Dtos;

public sealed record UpdateBookingServicesRequest(
    List<BookingServiceRequestDto> Services);