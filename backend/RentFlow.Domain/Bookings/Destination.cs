
using RentFlow.Domain.Bookings.ValueObjects;

namespace RentFlow.Domain.Bookings;

public class Destination
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Region { get; private set; } = null!;
    public int DistanceKm { get; private set; }
    public DestinationType Type { get; private set; }
    public decimal RouteSurcharge { get; private set; }
    public bool IsActive { get; private set; }
    
    private Destination() { }

    private Destination(
        string name,
        string region,
        int distanceKm,
        DestinationType type,
        decimal routeSurcharge)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Укажите название направления.", nameof(name));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Укажите регион.", nameof(region));

        if (distanceKm <= 0)
            throw new ArgumentOutOfRangeException(nameof(distanceKm), "Расстояние должно быть больше нуля.");

        if (routeSurcharge < 0)
            throw new ArgumentOutOfRangeException(nameof(routeSurcharge), "Доплата за маршрут не может быть отрицательной.");

        if (type is not DestinationType.Regional and not DestinationType.LongDistance)
            throw new ArgumentException("В каталоге допустимы только Regional и LongDistance.", nameof(type));

        if (type == DestinationType.Regional && distanceKm > 250)
            throw new ArgumentException("Для направления Regional расстояние должно быть не более 250 км.", nameof(distanceKm));

        if (type == DestinationType.LongDistance &&
            (distanceKm <= 250 || distanceKm > 1000))
            throw new ArgumentException("Для направления LongDistance расстояние должно быть больше 250 км и не более 1000 км.", nameof(distanceKm));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Region = region.Trim();
        DistanceKm = distanceKm;
        Type = type;
        RouteSurcharge = routeSurcharge;
        IsActive = true;
    }

    public static Destination Create(
        string name,
        string region,
        int distanceKm,
        DestinationType type,
        decimal routeSurcharge = 0)
    {
        return new Destination(
            name,
            region,
            distanceKm,
            type,
            routeSurcharge);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
