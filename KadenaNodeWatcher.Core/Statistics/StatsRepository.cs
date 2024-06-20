using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Statistics.Models.DbModels;

namespace KadenaNodeWatcher.Core.Statistics;

internal class StatsRepository(IDbConnectionFactory connectionFactory) : IStatsRepository
{
    public async Task AddStats(StatsDbModel statsDbModel)
    {
        using var conn = connectionFactory.Connection();
        await conn.ExecuteAsync("INSERT INTO Stats (Name, Content) VALUES (@Name, @Content)", statsDbModel);
    }
}