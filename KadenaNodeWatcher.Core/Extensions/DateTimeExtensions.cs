namespace KadenaNodeWatcher.Core.Extensions;

internal static class DateTimeExtensions
{
    internal static long ToUnixTimeSeconds(this DateTime dateTime)
        => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeSeconds();
    
    internal static long ToUnixTimeSecondsWithoutMinutes(this DateTime dateTime)
        => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).Date).ToUnixTimeSeconds();

    internal static DateTime UnixTimeToUtcDateTime(this long unixTime) 
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
    
    internal static DateTime UnixTimeToUtcDateTime(this int unixTime)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);

    internal static DateTime UnixTimeToLocalDateTime(this long unixTime)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();
    
    internal static DateTime UnixTimeToLocalDateTime(this int unixTime)
        => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();
}