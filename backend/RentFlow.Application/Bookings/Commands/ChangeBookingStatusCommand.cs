using MediatR;
using RentFlow.Domain.Bookings;

namespace RentFlow.Application.Bookings.Commands;

public record ChangeBookingStatusCommand(Guid BookingId, BookingStatus Status) : IRequest;