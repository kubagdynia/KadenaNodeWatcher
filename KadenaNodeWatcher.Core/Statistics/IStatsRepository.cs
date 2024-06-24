using KadenaNodeWatcher.Core.Statistics.Models.DbModels;

namespace KadenaNodeWatcher.Core.Statistics;

public interface IStatsRepository
{
    Task AddStats(StatsDbModel statsDbModel);
    
    Task AddOrUpdateStats(StatsDbModel statsDbModel);
}