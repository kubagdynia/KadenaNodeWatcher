using KadenaNodeWatcher.Core.Statistics.Models;

namespace KadenaNodeWatcher.Core.Statistics;

public interface IStats
{
    void AddStats(StatsName statsName, string message);
}