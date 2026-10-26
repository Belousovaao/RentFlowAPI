using RentFlow.Domain.Bookings;

namespace RentFlow.Api.Services.Requests;

public record ChangeBookingStatusRequest(BookingStatus Status);