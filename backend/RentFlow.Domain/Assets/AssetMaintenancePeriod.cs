namespace RentFlow.Domain.Assets;

public sealed class AssetMaintenancePeriod
{
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string? Reason { get; private set; }

    private AssetMaintenancePeriod() { }

    public AssetMaintenancePeriod(Guid assetId, DateTime startDate, DateTime endDate, string? reason)
    {
        if (assetId == Guid.Empty)
            throw new ArgumentException("AssetId is required.", nameof(assetId));
        if (endDate <= startDate)
            throw new ArgumentException("Maintenance end must be later than start.");

        Id = Guid.NewGuid();
        AssetId = assetId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }
}
