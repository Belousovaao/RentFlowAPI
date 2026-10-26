using MediatR;
using RentFlow.Application.Bookings.Dtos;

namespace RentFlow.Application.Bookings.Queries;

public record GetAdditionalServicesQuery()
    : IRequest<List<AdditionalServiceDto>>;