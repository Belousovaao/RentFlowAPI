namespace RentFlow.Domain.Bookings.ValueObjects;

public class BookingPrice
{
    public RentalPrice RentalPrice { get; private set; }
    public decimal Deposit { get; private set; }
    public decimal ServicesPrice { get; private set; }
    public decimal RouteSurcharge { get; private set; }
    private readonly List<BookingPenalty> _penalties = [];
    public IReadOnlyCollection<BookingPenalty> Penalties => _penalties.AsReadOnly();
    public decimal PenaltiesPrice => _penalties.Sum(x => x.Amount);
    /// <summary>
    /// Итоговая стоимость аренды без депозита.
    /// </summary>
    public decimal Total => RentalPrice.RentalPriceWithDiscount + ServicesPrice + RouteSurcharge + PenaltiesPrice;
    /// <summary>
    /// Итоговая сумма к оплате, включая депозит.
    /// </summary>
    public decimal TotalToPay => Total + Deposit;


    private BookingPrice()
    {
    }

    public BookingPrice(
        RentalPrice rentalPrice,
        decimal deposit,
        decimal servicesPrice = 0,
        decimal routeSurcharge = 0)
    {
        RentalPrice = rentalPrice;
        Deposit = deposit;
        ServicesPrice = servicesPrice;
        RouteSurcharge = routeSurcharge;
    }

    public void UpdateServicesPrice(decimal servicesPrice)
    {
        ServicesPrice = servicesPrice;
    }

    public void UpdateRouteSurcharge(decimal routeSurcharge)
    {
        RouteSurcharge = routeSurcharge;
    }

    public void UpdateRentalPrice(RentalPrice rentalPrice)
    {
        ArgumentNullException.ThrowIfNull(rentalPrice);
        RentalPrice.ApplySegments(rentalPrice.Segments);
    }

    public bool HasPenalty(BookingPenaltyType type) => _penalties.Any(x => x.Type == type);

    public void AddPenalty(BookingPenalty penalty)
    {
        ArgumentNullException.ThrowIfNull(penalty);
        if (HasPenalty(penalty.Type))
            return;
        _penalties.Add(penalty);
    }
}