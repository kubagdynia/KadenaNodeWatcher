using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Repositories.CommandQueries;
using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

internal class NodeRepository(
    IDbConnectionFactory connectionFactory, INodeCommandQueries nodeCommandQueries) : INodeRepository
{
    public async Task AddNode(NodeDbModel node)
    {
        using var conn = connectionFactory.Connection();

        await conn.ExecuteAsync(nodeCommandQueries.AddNode, node);

        await Task.CompletedTask;
    }

    public async Task AddNodes(IEnumerable<NodeDbModel> nodes)
    {
        using var conn = connectionFactory.Connection();
            
        conn.Open();

        var sqlTransaction = conn.BeginTransaction();

        await conn.ExecuteAsync(nodeCommandQueries.AddNode, nodes, transaction: sqlTransaction);

        sqlTransaction.Commit();

        await Task.CompletedTask;
    }

    public async Task<int> GetNumberOfNodes(DateTime date, bool? isOnline = null)
    {
        using var conn = connectionFactory.Connection();
    
        var unixTime = date.ToUnixTimeSecondsWithoutMinutes();
    
        object parameters;
            
        if (isOnline.HasValue)
        {
            parameters = new { date = unixTime, isOnline };
            return await conn.QueryFirstOrDefaultAsync<int>(nodeCommandQueries.GetNumberOfNodes(isOnline), parameters);
        }
    
        parameters = new { date = unixTime };
        return await conn.QueryFirstOrDefaultAsync<int>(nodeCommandQueries.GetNumberOfNodes(), parameters);
    }
    
    public async Task<IEnumerable<NumberOfNodesGroupedByDatesDb>> GetNumberOfNodesGroupedByDates(DateTime dateFrom, DateTime dateTo)
    {
        using var conn = connectionFactory.Connection();

        var unixTimeFrom = dateFrom.ToUnixTimeSecondsWithoutMinutes();
        var unixTimeTo = dateTo.ToUnixTimeSecondsWithoutMinutes();
        
        object parameters = new { dateFrom = unixTimeFrom, dateTo =  unixTimeTo };
        
        return await conn.QueryAsync<NumberOfNodesGroupedByDatesDb>(
            nodeCommandQueries.GetNumberOfNodesGroupedByDates(), parameters);
    }
    
    public async Task<IEnumerable<NumberOfNodesGroupedByCountryDb>> GetNumberOfNodesGroupedByCountry(
        DateTime date, bool? isOnline = null, string nodeVersion = null)
    {
        using var conn = connectionFactory.Connection();

        var unixTime = date.ToUnixTimeSecondsWithoutMinutes();

        object parameters;

        if (isOnline.HasValue)
        {
            parameters = new { date = unixTime, isOnline, nodeVersion };
            return await conn.QueryAsync<NumberOfNodesGroupedByCountryDb>(
                nodeCommandQueries.GetNumberOfNodesGroupedByCountry(isOnline, nodeVersion), parameters);
        }

        parameters = new { date = unixTime, nodeVersion };
        return await conn.QueryAsync<NumberOfNodesGroupedByCountryDb>(
            nodeCommandQueries.GetNumberOfNodesGroupedByCountry(isOnline: null, nodeVersion), parameters);
    }
    
    public async Task<IEnumerable<NumberOfNodesGroupedByVersionDb>> GetNumberOfNodesGroupedByVersion(
        DateTime date, bool? isOnline = null)
    {
        using var conn = connectionFactory.Connection();

        var unixTime = date.ToUnixTimeSecondsWithoutMinutes();

        object parameters;

        if (isOnline.HasValue)
        {
            parameters = new { date = unixTime, isOnline };
            return await conn.QueryAsync<NumberOfNodesGroupedByVersionDb>(
                nodeCommandQueries.GetNumberOfNodesGroupedByVersion(isOnline), parameters);
        }

        parameters = new { date = unixTime };
        return await conn.QueryAsync<NumberOfNodesGroupedByVersionDb>(
            nodeCommandQueries.GetNumberOfNodesGroupedByVersion(), parameters);
    }

    public async Task<IEnumerable<FullNodeDataDb>> GetNodes(DateTime date, bool? isOnline = null)
    {
        using var conn = connectionFactory.Connection();

        var unixTime = date.ToUnixTimeSecondsWithoutMinutes();
        
        object parameters;

        if (isOnline.HasValue)
        {
            parameters = new { date = unixTime, isOnline };
            return await conn.QueryAsync<FullNodeDataDb>(nodeCommandQueries.GetNodes(isOnline), parameters);
        }
        
        parameters = new { date = unixTime };
        return await conn.QueryAsync<FullNodeDataDb>(nodeCommandQueries.GetNodes(), parameters);
    }

    public async Task<IpGeolocationDb> GetIpGeolocationAsync(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            return await Task.FromResult<IpGeolocationDb>(null);
        }
        
        using var conn = connectionFactory.Connection();

        IpGeolocationDb ipGeolocationDb =
            await conn.QueryFirstOrDefaultAsync<IpGeolocationDb>(
                nodeCommandQueries.GetIpGeolocation, new { IpAddress = ip });

        return ipGeolocationDb;
    }

    public async Task<bool> IpGeolocationExistsAsync(string ip)
    {
        using var conn = connectionFactory.Connection();

        var exists =
            await conn.QueryFirstOrDefaultAsync<int>(nodeCommandQueries.IpGeolocationExists, new { IpAddress = ip });

        return exists == 1;
    }

    public async Task AddIpGeolocationAsync(IpGeolocationDb ipGeolocation)
    {
        if (ipGeolocation is null || await IpGeolocationExistsAsync(ipGeolocation.IpAddress))
        {
            await Task.CompletedTask;
        }
        
        using var conn = connectionFactory.Connection();
            
        conn.Open();

        var sqlTransaction = conn.BeginTransaction();

        await conn.ExecuteAsync(nodeCommandQueries.AddIpGeolocation, ipGeolocation, transaction: sqlTransaction);

        sqlTransaction.Commit();

        await Task.CompletedTask;
    }

    public async Task<IEnumerable<NodeDbModel>> GetNodesWithoutIpGeolocation(int numberOfRecords)
    {
        using var conn = connectionFactory.Connection();

        return await conn.QueryAsync<NodeDbModel>(nodeCommandQueries.GetNodesWithoutIpGeolocation(numberOfRecords));
    }
}