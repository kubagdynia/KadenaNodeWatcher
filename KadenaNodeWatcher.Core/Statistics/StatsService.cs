using KadenaNodeWatcher.Core.Statistics.Models;
using KadenaNodeWatcher.Core.Statistics.Models.DbModels;
using KadenaNodeWatcher.Core.Statistics.Repositories;

namespace KadenaNodeWatcher.Core.Statistics;

internal class StatsService(IStatsRepository repository) : IStatsService
{
    public void AddStats(StatsName statsName, string message)
    {
        var statsDbModel = new StatsDbModel
        {
            Name = statsName.ToString(),
            Content = string.IsNullOrEmpty(message) ? null :  message
        };
        repository.AddStats(statsDbModel);
    }

    public void AddOrUpdateStats(StatsName statsName, string message)
    {
        var statsDbModel = new StatsDbModel
        {
            Name = statsName.ToString(),
            Content = string.IsNullOrEmpty(message) ? null :  message
        };
        repository.AddOrUpdateStats(statsDbModel);
    }
}