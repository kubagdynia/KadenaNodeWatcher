using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Models.DbModels;
using KadenaNodeWatcher.Core.Repositories.CommandQueries;

namespace KadenaNodeWatcher.Core.Repositories;

public class NodeRepository : INodeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly INodeCommandQueries _nodeCommandQueries;

    public NodeRepository(IDbConnectionFactory connectionFactory, INodeCommandQueries nodeCommandQueries)
    {
        _connectionFactory = connectionFactory;
        _nodeCommandQueries = nodeCommandQueries;
    }

    public async Task AddNode(NodeDbModel node)
    {
        using var conn = _connectionFactory.Connection();

        await conn.ExecuteAsync(_nodeCommandQueries.AddNode, node);

        await Task.CompletedTask;
    }

    public async Task AddNodes(IEnumerable<NodeDbModel> nodes)
    {
        using var conn = _connectionFactory.Connection();
            
        conn.Open();

        var sqlTransaction = conn.BeginTransaction();

        await conn.ExecuteAsync(_nodeCommandQueries.AddNode, nodes, transaction: sqlTransaction);

        sqlTransaction.Commit();

        await Task.CompletedTask;
    }

    public async Task<int> CountNodes(DateTime date, bool? isOnline = null)
    {
        using var conn = _connectionFactory.Connection();

        var unixTime = GetUtcUnixTimeSeconds(date);

        object parameters;
            
        if (isOnline.HasValue)
        {
            parameters = new { date = unixTime, isOnline };
            return await conn.QueryFirstOrDefaultAsync<int>(_nodeCommandQueries.CountNodes(isOnline), parameters);
        }

        parameters = new { date = unixTime };
        return await conn.QueryFirstOrDefaultAsync<int>(_nodeCommandQueries.CountNodes(), parameters);

    }

    private long GetUtcUnixTimeSeconds(DateTime date)
        => new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc).Date).ToUnixTimeSeconds();
}