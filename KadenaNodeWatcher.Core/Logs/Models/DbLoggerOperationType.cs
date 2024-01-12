namespace KadenaNodeWatcher.Core.Logs.Models;

public enum DbLoggerOperationType
{
    None,
    GetNodesData
}

public static class DbLoggerOperationExtensions
{
    public static readonly Dictionary<DbLoggerOperationType, string> Types =
        Enum.GetValues(typeof(DbLoggerOperationType)).Cast<DbLoggerOperationType>()
            .ToDictionary(type => type, type => type.ToString());

    public static readonly Dictionary<DbLoggerOperationType, string> TypesUpperCase =
        Types.ToDictionary(k => k.Key, k => k.Value.ToUpper());

    public static string ToStringCode(this DbLoggerOperationType type) => Types[type];

    public static string ToStringCodeUpperCase(this DbLoggerOperationType type) => TypesUpperCase[type];
}