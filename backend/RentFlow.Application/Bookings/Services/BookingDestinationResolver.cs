using RentFlow.Application.Common.Exceptions;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Application.Bookings.Services;

public sealed class BookingDestinationResolver : IBookingDestinationResolver
{
    private readonly IDestinationRepository _destinations;

    public BookingDestinationResolver(IDestinationRepository destinations)
    {
        _destinations = destinations;
    }

    public async Task<ResolvedBookingDestination> ResolveAsync(
        DestinationType type,
        Guid? destinationId,
        string? customRouteDescription,
        CancellationToken cancellationToken = default)
    {
        switch (type)
        {
            case DestinationType.Local:
                EnsureNoCatalogDestination(destinationId);
                EnsureNoCustomDescription(customRouteDescription);

                return new ResolvedBookingDestination(
                    BookingDestination.Local(),
                    0m);

            case DestinationType.Regional:
            case DestinationType.LongDistance:
            {
                if (destinationId is null || destinationId == Guid.Empty)
                {
                    throw new BusinessRuleViolationException(
                        "Для выбранного типа маршрута необходимо выбрать город назначения.");
                }

                EnsureNoCustomDescription(customRouteDescription);

                var destination = await _destinations.GetActiveByIdAsync(
                    destinationId.Value,
                    cancellationToken);

                if (destination is null)
                {
                    throw new BusinessRuleViolationException("Направление не найдено или больше недоступно.");
                }

                if (destination.Type != type)
                {
                    throw new BusinessRuleViolationException("Выбранный город не соответствует типу маршрута.");
                }

                ValidateDistance(destination.DistanceKm, type);

                return new ResolvedBookingDestination(
                    BookingDestination.CatalogDestination(
                        destination.Id,
                        destination.Type,
                        destination.Name),
                    destination.RouteSurcharge);
            }

            case DestinationType.Other:
                EnsureNoCatalogDestination(destinationId);

                if (string.IsNullOrWhiteSpace(customRouteDescription))
                {
                    throw new BusinessRuleViolationException("Необходимо описать планируемый маршрут.");
                }

                return new ResolvedBookingDestination(
                    BookingDestination.CustomRoute(customRouteDescription),
                    0m);

            default:
                throw new BusinessRuleViolationException("Указан неизвестный тип маршрута.");
        }
    }

    private static void EnsureNoCatalogDestination(Guid? destinationId)
    {
        if (destinationId.HasValue)
        {
            throw new BusinessRuleViolationException("Для выбранного типа маршрута нельзя указывать город из каталога.");
        }
    }

    private static void EnsureNoCustomDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            throw new BusinessRuleViolationException("Для выбранного типа маршрута нельзя указывать произвольное описание.");
        }
    }

    private static void ValidateDistance(int distanceKm, DestinationType type)
    {
        var isValid = type switch
        {
            DestinationType.Regional => distanceKm > 0 && distanceKm <= 250,
            DestinationType.LongDistance => distanceKm > 250 && distanceKm <= 1000,
            _ => false
        };

        if (!isValid)
        {
            throw new BusinessRuleViolationException("Расстояние направления не соответствует выбранному типу маршрута.");
        }
    }
}