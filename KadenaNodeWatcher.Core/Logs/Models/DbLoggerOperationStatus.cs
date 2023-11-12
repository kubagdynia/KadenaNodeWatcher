namespace KadenaNodeWatcher.Core.Logs.Models;

public enum DbLoggerOperationStatus
{
    Info,
    Warning,
    Error,
    Success
}

public static class DbLoggerOperationStatusExtensions
{
    public static readonly Dictionary<DbLoggerOperationStatus, string> Statuses =
        Enum.GetValues(typeof(DbLoggerOperationStatus)).Cast<DbLoggerOperationStatus>()
            .ToDictionary(status => status, status => status.ToString());

    public static readonly Dictionary<DbLoggerOperationStatus, string> StatusesUpperCase =
        Statuses.ToDictionary(k => k.Key, k => k.Value.ToUpper());

    public static string ToStringCode(this DbLoggerOperationStatus status) => Statuses[status];

    public static string ToStringCodeUpperCase(this DbLoggerOperationStatus status) => StatusesUpperCase[status];
}