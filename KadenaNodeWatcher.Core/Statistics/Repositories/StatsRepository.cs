using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Statistics.Models.DbModels;

namespace KadenaNodeWatcher.Core.Statistics.Repositories;

internal class StatsRepository(IDbConnectionFactory connectionFactory) : IStatsRepository
{
    // Add the statistics
    public async Task AddStats(StatsDbModel statsDbModel)
    {
        using var conn = connectionFactory.Connection();
        await conn.ExecuteAsync("INSERT INTO Stats (Name, Content) VALUES (@Name, @Content)", statsDbModel);
    }

    // Add or update the statistics
    public async Task AddOrUpdateStats(StatsDbModel statsDbModel)
    {
        using var conn = connectionFactory.Connection();
        await conn.ExecuteAsync(
            """
            INSERT OR IGNORE INTO Stats (Name, Content) VALUES (@Name, @Content);
            UPDATE Stats SET Content = @Content, Timestamp = (strftime('%s', 'now')) WHERE Name = @Name;
            """, statsDbModel);
    }
    
    // Get the statistics
    public async Task<IEnumerable<StatsDbModel>> GetStats()
    {
        using var conn = connectionFactory.Connection();
        return await conn.QueryAsync<StatsDbModel>("SELECT * FROM Stats ORDER BY Name");
    }
}