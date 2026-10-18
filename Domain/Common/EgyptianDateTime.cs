using Domain.Constants;

namespace Domain.Common;

/// <summary>Converts instants between UTC storage and Egypt local time.</summary>
public static class EgyptianDateTime
{
    private static readonly TimeZoneInfo Cairo = ResolveTimeZone();
    public static DateTimeOffset Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Cairo);
    public static DateTimeOffset ToCairo(DateTimeOffset instant) => TimeZoneInfo.ConvertTime(instant, Cairo);
    public static DateTimeOffset ToUtc(DateTimeOffset instant) => instant.ToUniversalTime();
    public static TimeZoneInfo TimeZone => Cairo;
    private static TimeZoneInfo ResolveTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(DomainConstants.EgyptTimeZoneId); }
        catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); }
    }
}
