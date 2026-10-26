namespace RentFlow.Domain.Bookings.ValueObjects;

public class RentalPrice
{
    private readonly List<RentalPriceSegment> _segments = [];
    public IReadOnlyCollection<RentalPriceSegment> Segments => _segments.OrderBy(x => x.Order).ToList().AsReadOnly();
    public decimal BaseRentalPrice => _segments.Sum(x => x.BaseRentalPrice);
    public decimal DiscountAmount => _segments.Sum(x => x.DiscountAmount);
    public decimal RentalPriceWithDiscount => _segments.Sum(x => x.Total);
    public RentalPriceSegment LastSegment => _segments.OrderBy(x => x.Order).Last();

    private RentalPrice()
    {
    }

    public RentalPrice(RentalPriceSegment initialSegment)
    {
        ArgumentNullException.ThrowIfNull(initialSegment);
        initialSegment.SetOrder(0);
        _segments.Add(initialSegment);
    }

    private RentalPrice(IEnumerable<RentalPriceSegment> segments)
    {
        ArgumentNullException.ThrowIfNull(segments); 
        var segmentList = segments.ToList(); 

        if (segmentList.Count == 0) 
        {
            throw new ArgumentException( "Rental price must contain at least one segment.", nameof(segments));
        }
        for (var i = 0; i < segmentList.Count; i++)
        {
            segmentList[i].SetOrder(i);
            _segments.Add(segmentList[i]);
        }
    }

    public RentalPrice AddSegment(RentalPriceSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        segment.SetOrder(_segments.Count);
        _segments.Add(segment);

        return this;
    }

    public static RentalPrice FromSegments(IEnumerable<RentalPriceSegment> segments) 
    { 
        return new RentalPrice(segments); 
    }

    public RentalPrice Copy()
    {
        return new RentalPrice(_segments
                .OrderBy(x => x.Order)
                .Select(segment => segment.Copy()));
    }

    public void ApplySegments(IEnumerable<RentalPriceSegment> newSegments)
    {
        ArgumentNullException.ThrowIfNull(newSegments);

        var segments = newSegments
            .OrderBy(x => x.Order)
            .ToList();

        if (segments.Count == 0)
            throw new ArgumentException("Rental price must contain at least one segment.", nameof(newSegments));

        var existingByOrder = _segments.ToDictionary(x => x.Order);

        var newOrders = segments
            .Select(x => x.Order)
            .ToHashSet();

        // Удаляем сегменты, которых больше нет
        _segments.RemoveAll(x => !newOrders.Contains(x.Order));

        foreach (var newSegment in segments)
        {
            // Сегмент уже существует — обновляем его,
            // сохраняя его Id и EF tracking.
            if (existingByOrder.TryGetValue(newSegment.Order, out var existingSegment))
            {
                existingSegment.Update(
                    days: newSegment.Days,
                    durationDiscountPercent: newSegment.DurationDiscountPercent,
                    discountedDailyPrice: newSegment.DiscountedDailyPrice,
                    baseRentalPrice: newSegment.BaseRentalPrice,
                    discountAmount: newSegment.DiscountAmount,
                    total: newSegment.Total,
                    appliedForDays: newSegment.AppliedForDays);

                continue;
            }

            // Новый сегмент
            newSegment.SetOrder(newSegment.Order);
            _segments.Add(newSegment);
        }
    }

}