using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Application.Bookings.Dtos;

public sealed record AdditionalServiceDto(
    Guid Id,
    string Name,
    ServiceType Type,
    decimal Price
);