using RentFlow.Domain.Bookings.Services;

namespace RentFlow.Domain.Bookings;

public class BookingService
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public ServiceType Type { get; private set; }
    public Guid AdditionalServiceId { get; private set; }

    public string Name { get; private set; }

    /// <summary>
    /// Стоимость услуги на момент бронирования.
    /// Даже если потом тариф изменится,
    /// старая бронь останется неизменной.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Дополнительный параметр услуги.
    /// Например:
    /// Early Pickup -> 09:00
    /// Late Return -> 18:00
    /// Для остальных услуг = null
    /// </summary>
    public TimeOnly? SelectedTime { get; private set; }


    private BookingService()
    {
    }

    public BookingService(AdditionalService service, ServiceType serviceType, decimal price, TimeOnly? selectedTime = null)
    {
        Id = Guid.NewGuid();
        AdditionalServiceId = service.Id;
        Type = serviceType;
        Name = service.Name;
        Price = price;
        SelectedTime = selectedTime;
    }

    public void SetBookingId(Guid bookingId)
    {
        BookingId = bookingId;
    }
}