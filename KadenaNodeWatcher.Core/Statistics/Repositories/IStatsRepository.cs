using KadenaNodeWatcher.Core.Statistics.Models.DbModels;

namespace KadenaNodeWatcher.Core.Statistics.Repositories;

internal interface IStatsRepository
{
    // Add the statistics
    Task AddStats(StatsDbModel statsDbModel);
    
    // Add or update the statistics
    Task AddOrUpdateStats(StatsDbModel statsDbModel);
    
    // Get the statistics
    Task<IEnumerable<StatsDbModel>> GetStats();
}