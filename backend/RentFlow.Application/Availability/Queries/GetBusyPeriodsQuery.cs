using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Availability.Queries;

public sealed record GetBusyPeriodsQuery(Guid AssetId, Guid? ExcludeBookingId) : IRequest<List<BusyPeriodDto>>;