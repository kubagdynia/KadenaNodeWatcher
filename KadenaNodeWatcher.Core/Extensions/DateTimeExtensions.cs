namespace KadenaNodeWatcher.Core.Extensions;

internal static class DateTimeExtensions
{
    internal static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeSeconds();
    }

    internal static DateTime UnixTimeToUtcDateTime(this long unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
    }
    
    internal static DateTime UnixTimeToUtcDateTime(this int unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
    }
    
    internal static DateTime UnixTimeToLocalDateTime(this long unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();
    }
    
    internal static DateTime UnixTimeToLocalDateTime(this int unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();
    }
}