using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Repositories.CommandQueries;
using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

internal class NodeRepository : INodeRepository
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

    public async Task<IpGeolocationDb> GetIpGeolocationAsync(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            return await Task.FromResult<IpGeolocationDb>(null);
        }
        
        using var conn = _connectionFactory.Connection();

        IpGeolocationDb ipGeolocationDb =
            await conn.QueryFirstOrDefaultAsync<IpGeolocationDb>(
                _nodeCommandQueries.GetIpGeolocation, new { IpAddress = ip });

        return ipGeolocationDb;
    }

    public async Task<bool> IpGeolocationExistsAsync(string ip)
    {
        using var conn = _connectionFactory.Connection();

        var exists =
            await conn.QueryFirstOrDefaultAsync<int>(_nodeCommandQueries.IpGeolocationExists, new { IpAddress = ip });

        return exists == 1;
    }

    public async Task AddIpGeolocationAsync(IpGeolocationDb ipGeolocation)
    {
        if (ipGeolocation is null || await IpGeolocationExistsAsync(ipGeolocation.IpAddress))
        {
            await Task.CompletedTask;
        }
        
        using var conn = _connectionFactory.Connection();
            
        conn.Open();

        var sqlTransaction = conn.BeginTransaction();

        await conn.ExecuteAsync(_nodeCommandQueries.AddIpGeolocation, ipGeolocation, transaction: sqlTransaction);

        sqlTransaction.Commit();

        await Task.CompletedTask;
    }

    private long GetUtcUnixTimeSeconds(DateTime date)
        => new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Utc).Date).ToUnixTimeSeconds();
}