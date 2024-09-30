using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Statistics.Models;
using KadenaNodeWatcher.Core.Statistics.Models.DbModels;
using KadenaNodeWatcher.Core.Statistics.Models.Dto;
using KadenaNodeWatcher.Core.Statistics.Repositories;

namespace KadenaNodeWatcher.Core.Statistics;

internal class StatsService(IStatsRepository repository) : IStatsService
{
    // Add the statistics
    public async void AddStats(StatsName statsName, string message)
    {
        var statsDbModel = new StatsDbModel
        {
            Name = statsName.ToString(),
            Content = string.IsNullOrEmpty(message) ? null :  message
        };
        await repository.AddStats(statsDbModel);
    }

    // Add or update the statistics
    public async void AddOrUpdateStats(StatsName statsName, string message)
    {
        var statsDbModel = new StatsDbModel
        {
            Name = statsName.ToString(),
            Content = string.IsNullOrEmpty(message) ? null :  message
        };
        await repository.AddOrUpdateStats(statsDbModel);
    }
    
    // Get the statistics
    public async Task<IEnumerable<StatsDto>> GetStats()
    {
        IEnumerable<StatsDbModel> statsDbModel = await repository.GetStats();
        return statsDbModel.Select(x => new StatsDto
        {
            Name = x.Name,
            Content = x.Content,
            Updated = x.Timestamp.UnixTimeToUtcDateTime()
        });
    }
}