namespace RentFlow.Domain.Bookings.ValueObjects;

public sealed class BookingDestination
{
    public Guid? DestinationId { get; private set; }

    public DestinationType Type { get; private set; }

    public string? CustomRouteDescription { get; private set; }

    public RouteReviewStatus ReviewStatus { get; private set; }

    public string? DestinationNameSnapshot { get; private set; }

    private BookingDestination() { }

    private BookingDestination(
        Guid? destinationId,
        DestinationType type,
        string? customRouteDescription,
        RouteReviewStatus reviewStatus,
        string? destinationNameSnapshot)
    {
        DestinationId = destinationId;
        Type = type;
        CustomRouteDescription = customRouteDescription;
        ReviewStatus = reviewStatus;
        DestinationNameSnapshot = destinationNameSnapshot;
    }

    public static BookingDestination Local()
    {
        return new BookingDestination(
            destinationId: null,
            type: DestinationType.Local,
            customRouteDescription: null,
            reviewStatus: RouteReviewStatus.NotRequired,
            destinationNameSnapshot: null);
    }

    public static BookingDestination CatalogDestination(
        Guid destinationId,
        DestinationType type,
        string destinationName)
    {
        if (destinationId == Guid.Empty)
            throw new ArgumentException("Не указан идентификатор направления.", nameof(destinationId));

        if (type is not DestinationType.Regional and not DestinationType.LongDistance)
        {
            throw new ArgumentException("Для каталожного направления допустимы только Regional и LongDistance.", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(destinationName))
            throw new ArgumentException("Не указано название направления.", nameof(destinationName));

        return new BookingDestination(
            destinationId,
            type,
            customRouteDescription: null,
            reviewStatus: RouteReviewStatus.NotRequired,
            destinationNameSnapshot: destinationName.Trim());
    }

    public static BookingDestination CustomRoute(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Необходимо описать планируемый маршрут.", nameof(description));

        return new BookingDestination(
            destinationId: null,
            type: DestinationType.Other,
            customRouteDescription: description.Trim(),
            reviewStatus: RouteReviewStatus.Pending,
            destinationNameSnapshot: null);
    }

    public void ApproveReview()
    {
        EnsureReviewPending();
        ReviewStatus = RouteReviewStatus.Approved;
    }

    public void RejectReview()
    {
        EnsureReviewPending();
        ReviewStatus = RouteReviewStatus.Rejected;
    }

    private void EnsureReviewPending()
    {
        if (Type != DestinationType.Other || ReviewStatus != RouteReviewStatus.Pending)
        {
            throw new InvalidOperationException("Проверка доступна только для маршрута, ожидающего решения менеджера.");
        }
    }
}

public enum DestinationType
{
    Local,
    Regional,
    LongDistance,
    Other
}

public enum RouteReviewStatus
{
    NotRequired,
    Pending,
    Approved,
    Rejected
}