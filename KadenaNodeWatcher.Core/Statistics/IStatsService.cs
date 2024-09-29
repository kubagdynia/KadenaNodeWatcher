using KadenaNodeWatcher.Core.Statistics.Models;

namespace KadenaNodeWatcher.Core.Statistics;

public interface IStatsService
{
    void AddStats(StatsName statsName, string message);
    
    void AddOrUpdateStats(StatsName statsName, string message);
}