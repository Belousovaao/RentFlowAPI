namespace RentFlow.Application.Interfaces;
public interface IDateTimeProvider
{
    TimeZoneInfo BusinessTimeZone { get; }
    DateTimeOffset UtcNow { get; }
    DateTime ToUtc(DateOnly date, TimeOnly? time);
    DateTime ToLocal(DateTime utc);

}