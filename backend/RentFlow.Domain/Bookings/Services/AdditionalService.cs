namespace RentFlow.Domain.Bookings.Services;

public class AdditionalService
{
    public Guid Id { get; private set; }

    public ServiceType Type { get; private set; }

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private AdditionalService() {}

    public AdditionalService(
        ServiceType type,
        string name,
        decimal price,
        int displayOrder = 0)
    {
        Id = Guid.NewGuid();
        Type = type;
        Name = name;
        Price = price;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void Update(
        ServiceType type,
        string name,
        decimal price,
        int displayOrder)
    {
        Type = type;
        Name = name;
        Price = price;
        DisplayOrder = displayOrder;
    }
    public void SetDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    public void Disable()
    {
        IsActive = false;
    }
}

public enum ServiceType
{
    Regular = 0,
    EarlyPickup = 1,
    LateReturn = 2
}