using RentFlow.Application.Interfaces;

public class KrasnoyarskDateTimeProvider : IDateTimeProvider
{
    private static readonly TimeOnly DefaultTime = new(12, 0);

    public TimeZoneInfo BusinessTimeZone => TimeZoneInfo.FindSystemTimeZoneById("Asia/Krasnoyarsk");
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTime ToUtc(DateOnly date, TimeOnly? time)
    {
        var local = date.ToDateTime(time ?? DefaultTime);

        return TimeZoneInfo.ConvertTimeToUtc(
            local,
            BusinessTimeZone);
    }

    public DateTime ToLocal(DateTime utc)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            utc,
            BusinessTimeZone);
    }
}