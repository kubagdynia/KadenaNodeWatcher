using KadenaNodeWatcher.Core.Statistics.Models;
using KadenaNodeWatcher.Core.Statistics.Models.Dto;

namespace KadenaNodeWatcher.Core.Statistics;

public interface IStatsService
{
    // Add the statistics
    void AddStats(StatsName statsName, string message);
    
    // Add or update the statistics
    void AddOrUpdateStats(StatsName statsName, string message);
    
    // Get the statistics
    Task<IEnumerable<StatsDto>> GetStats();
}