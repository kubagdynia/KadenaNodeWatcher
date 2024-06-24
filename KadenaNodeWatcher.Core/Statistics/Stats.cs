using KadenaNodeWatcher.Core.Statistics.Models;
using KadenaNodeWatcher.Core.Statistics.Models.DbModels;

namespace KadenaNodeWatcher.Core.Statistics;

public class Stats(IStatsRepository repository) : IStats
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