namespace KadenaNodeWatcher.Core.Statistics.Models;

public enum StatsName
{
    None,
    LastCheckingNodesData,
    LastCheckingIpGeolocations
}

public static class StatsNameExtensions
{
    private static readonly Dictionary<StatsName, string> Types =
        Enum.GetValues(typeof(StatsName)).Cast<StatsName>()
            .ToDictionary(type => type, type => type.ToString());

    private static readonly Dictionary<StatsName, string> TypesUpperCase =
        Types.ToDictionary(k => k.Key, k => k.Value.ToUpper());

    public static string ToStringCode(this StatsName type) => Types[type];

    public static string ToStringCodeUpperCase(this StatsName type) => TypesUpperCase[type];
}