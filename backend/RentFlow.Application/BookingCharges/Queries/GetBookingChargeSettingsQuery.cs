using MediatR;
using RentFlow.Application.BookingCharges.Dtos;
namespace RentFlow.Application.BookingCharges.Queries;
public sealed record GetBookingChargeSettingsQuery : IRequest<BookingChargeSettingsDto>;