
using MediatR;
using RentFlow.Application.Payments.Dtos;

namespace RentFlow.Application.Payments.Queries;

public sealed record GetBookingPaymentSummaryQuery(Guid BookingId) : IRequest<BookingPaymentSummaryDto>;
