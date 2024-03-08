using IpGeolocation.Models;
using IpGeolocation.Services;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Helpers;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Logs.Models;
using KadenaNodeWatcher.Core.Models;
using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Repositories;
using KadenaNodeWatcher.Core.Repositories.DbModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.Core.Services;

internal class KadenaNodeWatcherService(
    IChainwebNodeService chainwebNodeService,
    IIpGeolocationService ipGeolocationService,
    INodeRepository nodeRepository,
    IOptions<ChainwebSettings> chainwebSettings,
    IAppLogger appLogger,
    ILogger<ChainwebNodeService> logger)
    : IKadenaNodeWatcherService
{
    private readonly ChainwebSettings _chainwebSettings = chainwebSettings.Value;

    public async Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false, CancellationToken ct = default)
    {
        List<Peer> uniquePeers = [];

        var response = await chainwebNodeService.GetCutNetworkPeerInfoAsync(hostName, ct);
        uniquePeers.AddRange(response.Page.Items);
        
        var ip = IpHelper.GetIp(hostName);
        
        IpGeolocationModel ipGeolocation = null;
        if (checkIpGeolocation)
        {
            var ipGeolocationDb = await nodeRepository.GetIpGeolocationAsync(ip);

            if (ipGeolocationDb is null)
            {
                ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(ip);
                if (ipGeolocation is not null)
                {
                    await nodeRepository.AddIpGeolocationAsync(ipGeolocation.ToDbModel());
                }
            }
            else
            {
                ipGeolocation = ipGeolocationDb.ToApiModel();
            }
        }
        
        var nodeDataResponse = new NodeDataResponse
        {
            HostName = hostName,
            Ip = ip,
            ChainwebNodeVersion = response.ResponseHeaders.ChainwebNodeVersion,
            ServerTimestamp = response.ResponseHeaders.ServerTimestamp,
            ServerDateTime = response.ResponseHeaders.ServerTimestamp.UnixTimeToUtcDateTime(),
            UniquePeers = uniquePeers.Count,
            IpGeo = checkIpGeolocation && ipGeolocation is not null ? IpGeo.CreateIpGeo(ipGeolocation) : null  
        };

        return await Task.FromResult(nodeDataResponse);
    }

    public async Task CollectNodeData(bool checkIpGeolocation = false, CancellationToken ct = default)
    {
        appLogger.AddInfoLog("START");
        
        var numberOfNodes = await nodeRepository.GetNumberOfNodes(DateTime.Now);
        
        if (numberOfNodes > 0)
        {
            appLogger.AddInfoLog($"Nodes data has already been collected today: {numberOfNodes}.", DbLoggerOperationType.GetNodesData);
            await Task.CompletedTask;
            return;
        }
        
        var (uniquePeers, rootUniquePeers) = await CollectDataFromRootNodes(ct);
        
        // Returns a full or partial list of child nodes
        var peers = PreparePeers(rootUniquePeers);
        
        await CollectDataFromChildNodes(peers, uniquePeers, ct);
        
        uniquePeers.AddIpAddress();
        
        uniquePeers.Sort((peer, peer1) =>
            string.Compare(peer.Address.Hostname, peer1.Address.Hostname, StringComparison.Ordinal));
        
        var(online, offline) = await CheckNodesAreOnline(uniquePeers, ct);
        
        await SaveNodes(uniquePeers);

        if (checkIpGeolocation)
        {
            await CheckIpGeolocation(uniquePeers);
        }

        appLogger.AddInfoLog($"FINISH - number of nodes: {uniquePeers.Count}, Online: {online}, Offline: {offline}");

        await Task.CompletedTask;
    }

    public async Task<int> GetNumberOfNodes(DateTime dateTime, bool? isOnline = null)
    {
        return await nodeRepository.GetNumberOfNodes(dateTime, isOnline);
    }

    public async Task<IEnumerable<NumberOfNodesGroupedByDatesDto>> GetNumberOfNodesGroupedByDates(DateTime dateFrom, DateTime dateTo)
    {
        var numberOfNodesGroupdeByDates =
            await nodeRepository.GetNumberOfNodesGroupedByDates(dateFrom, dateTo);
        
        return numberOfNodesGroupdeByDates.Select(
            item => new NumberOfNodesGroupedByDatesDto
            {
                Date = item.Date.UnixTimeToUtcDateTime(),
                TotalCount = item.TotalCount,
                Online = item.Online,
                Offline = item.Offline
            }).ToList();
    }

    public async Task<IEnumerable<NumberOfNodesGroupedByCountryDto>> GetNumberOfNodesGroupedByCountry(
        DateTime dateTime, bool? isOnline = null)
    {
        var nodesGroupedByCountry =
            await nodeRepository.GetNumberOfNodesGroupedByCountry(dateTime, isOnline);

        return nodesGroupedByCountry.Select(
            item => new NumberOfNodesGroupedByCountryDto
            {
                CountryName = item.CountryName,
                CountryCode = item.CountryCode,
                Count = item.Count
            }).ToList();
    }
    
    public async Task<IEnumerable<NumberOfNodesGroupedByVersionDto>> GetNumberOfNodesGroupedByVersion(
        DateTime dateTime, bool? isOnline = null)
    {
        var nodesGroupedByVersion =
            await nodeRepository.GetNumberOfNodesGroupedByVersion(dateTime, isOnline);

        return nodesGroupedByVersion.Select(
            item => new NumberOfNodesGroupedByVersionDto
            {
                NodeVersion = item.NodeVersion,
                Date = item.Date.UnixTimeToUtcDateTime(),
                Count = item.Count
            }).ToList();
    }

    public async Task<IEnumerable<FullNodeDataDto>> GetNodes(DateTime date, bool? isOnline = null)
    {
        var nodes = await nodeRepository.GetNodes(date, isOnline);
        
        return nodes.Select(
            node => new FullNodeDataDto
            {
                Id = node.Id,
                Created = node.Created.UnixTimeToUtcDateTime(),
                IpAddress = node.IpAddress,
                Hostname = node.Hostname,
                Port = node.Port,
                IsOnline = node.IsOnline,
                NodeVersion = node.NodeVersion,
                CountryName = node.CountryName,
                CountryCode = node.CountryCode,
                City = node.City,
                ContinentCode = node.ContinentCode,
                Org = node.Org
            }
        ).ToList();
    }

    public async Task CollectNodeIpGeolocations(int numberOfRecords)
    {
        var nodes = await nodeRepository.GetNodesWithoutIpGeolocation(numberOfRecords);
        
        foreach (var node in nodes)
        {
            if (!await nodeRepository.IpGeolocationExistsAsync(node.IpAddress))
            {
                var ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(node.IpAddress);
                if (ipGeolocation is not null)
                {
                    await nodeRepository.AddIpGeolocationAsync(ipGeolocation.ToDbModel());
                }
            }
        }
        
        await Task.CompletedTask;
    }
    
    private async Task<(ConcurrentList<Peer>, List<Peer>)> CollectDataFromRootNodes(CancellationToken ct)
    {
        appLogger.AddInfoLog("Start collecting data from root nodes...", DbLoggerOperationType.GetNodesData);
        
        ConcurrentList<Peer> uniquePeers = [];
        List<Peer> rootUniquePeers = [];
        
        var selectedRootNode = _chainwebSettings.GetSelectedRootNode();
        
        try
        {
            var selectedRootNodeResponse = await chainwebNodeService.GetCutNetworkPeerInfoAsync(selectedRootNode, ct);
            rootUniquePeers.AddRange(selectedRootNodeResponse.Page.Items);
            uniquePeers.AddRange(selectedRootNodeResponse.Page.Items);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred.");
        }
    
        var notSelectedRootNodes = _chainwebSettings.GetNotSelectedRootNodes();
        foreach (var notSelectedRootNode in notSelectedRootNodes)
        {
            try
            {
                var res = await chainwebNodeService.GetCutNetworkPeerInfoAsync(notSelectedRootNode, ct);
                if (rootUniquePeers.Count == 0)
                {
                    rootUniquePeers.AddRange(res.Page.Items);
                }
                uniquePeers.AddUniqueAddress(res.Page.Items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred.");
            }
        }
        
        appLogger.AddInfoLog($"Finish. Unique nodes: {uniquePeers.Count}", DbLoggerOperationType.GetNodesData);

        return (uniquePeers, rootUniquePeers);
    }
    
    private async Task CollectDataFromChildNodes(List<Peer> peers,
        ConcurrentList<Peer> uniquePeers, CancellationToken ct)
    {
        appLogger.AddInfoLog($"Start collecting node data from child nodes...",
            DbLoggerOperationType.GetNodesData);
        
        // Limiting the maximum degree of parallelism to 3
        var parallelOptions = CreateParallelOptions(maxDegreeOfParallelism: 3);
        
        await Parallel.ForEachAsync(peers, parallelOptions, async (peer, _) =>
        {
            await GetUniquePeers(peer, uniquePeers, ct);
        });
        appLogger.AddInfoLog($"Finish. Unique nodes: {uniquePeers.Count}.",
            DbLoggerOperationType.GetNodesData);
    }

    private List<Peer> PreparePeers(List<Peer> items)
    {
        var networkConfig = _chainwebSettings.GetSelectedNetworkConfig();
        
        // Sometimes there is a need not to check all the nodes and only a part, which can be selected randomly.
        // We rely on the configuration that was specified in the ChildNodes section.
        
        if (items == null || networkConfig?.ChildNodes == null || networkConfig.ChildNodes.Count >= items.Count)
        {
            return items;
        }

        var numberOfChildNodes = networkConfig.ChildNodes.Count <= 0 ? items.Count : networkConfig.ChildNodes.Count;

        // If this option is enabled then select random child nodes
        if (!networkConfig.ChildNodes.RandomPick)
        {
            return items.Take(numberOfChildNodes).ToList();
        }

        // Return random child nodes
        return items.GetRandomElements(numberOfChildNodes);
    }
    
    private async Task GetUniquePeers(Peer peer, ConcurrentList<Peer> uniquePeers, CancellationToken ct = default)
    {
        var peers = uniquePeers.Where(c => c.Address.Hostname == peer.Address.Hostname).ToList();
        
        GetCutNetworkPeerInfoResponse response;
        try
        {
            response =
                await chainwebNodeService.GetCutNetworkPeerInfoAsync(
                    $"https://{peer.Address.Hostname}:{peer.Address.Port}", ct);
        }
        catch (Exception)
        {
            foreach (var peer2 in peers)
            {
                peer2.IsOnline = false;
            }

            return;
        }
        
        if (response is null)
        {
            foreach (var peer2 in peers)
            {
                peer2.IsOnline = false;
            }

            return;
        }

        foreach (var peer2 in peers)
        {
            peer2.IsOnline = true;
            peer2.ChainwebNodeVersion = response.ResponseHeaders?.ChainwebNodeVersion;
        }

        uniquePeers.AddUniqueAddress(response.Page.Items);
    }

    /// <summary>
    /// Check if the node is active under the specified port
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="ct">Cancellation token</param>
    private async Task IsOnline(Peer peer, CancellationToken ct = default)
    {
        if (peer is null) return;

        try
        {
            var response =
                await chainwebNodeService.GetCutAsync($"https://{peer.Address.Hostname}:{peer.Address.Port}", ct);

            if (response is null)
            {
                peer.IsOnline = false;
                return;
            }

            peer.IsOnline = true;
            peer.ChainwebNodeVersion = response.ResponseHeaders.ChainwebNodeVersion;

        }
        catch (Exception)
        {
            peer.IsOnline = false;
        }
    }
    
    private async Task<(int, int)> CheckNodesAreOnline(ConcurrentList<Peer> peers, CancellationToken ct)
    {
        appLogger.AddInfoLog($"Start checking, whether nodes are online and what is their version...",
            DbLoggerOperationType.GetNodesData);
        
        var uniquePeersToCheck = peers.Where(c => c.IsOnline is null).ToList();
        
        foreach (var value1 in uniquePeersToCheck)
        {
            value1.IsOnline = true;
        }
        
        // Limiting the maximum degree of parallelism to 3
        var parallelOptions = CreateParallelOptions(maxDegreeOfParallelism: 3);
        
        // The code will be executed in parallel by up to three threads
        await Parallel.ForEachAsync(uniquePeersToCheck, parallelOptions, async (peer, _) =>
        {
            await IsOnline(peer, ct);
        });
        
        var online = peers.Count(c => c.IsOnline.HasValue && c.IsOnline.Value);
        var offline = peers.Count - online;
        
        appLogger.AddInfoLog($"Finish. Online: {online}, Offline: {offline}",
            DbLoggerOperationType.GetNodesData);
        
        return (online, offline);
    }
    
    private async Task CheckIpGeolocation(ConcurrentList<Peer> peers)
    {
        appLogger.AddInfoLog($"Start checking IP geolocation...", DbLoggerOperationType.GetNodesData);

        foreach (var peer in peers)
        {
            var ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(peer.Address.Ip);
            if (!await nodeRepository.IpGeolocationExistsAsync(peer.Address.Ip))
            {
                if (ipGeolocation is not null)
                {
                    await nodeRepository.AddIpGeolocationAsync(ipGeolocation.ToDbModel());
                }
            }
        }
    }
    
    private async Task SaveNodes(ConcurrentList<Peer> uniquePeers)
    {
        // Prepare nodes information to be stored in the database 
        var nodeList = uniquePeers.Select(
            peer => NodeDbModel.CreateNodeDbModel(
                peer.Address.Ip,
                peer.Address.Hostname,
                peer.Address.Port,
                peer.IsOnline,
                peer.ChainwebNodeVersion)
        ).ToList();

        // Saving nodes info in the db
        await nodeRepository.AddNodes(nodeList);
    }

    /// <summary>
    /// Limiting the maximum degree of parallelism to 3
    /// </summary>
    /// <param name="maxDegreeOfParallelism"></param>
    private ParallelOptions CreateParallelOptions(int maxDegreeOfParallelism)
        => new()
        {
            MaxDegreeOfParallelism = Math.Min(maxDegreeOfParallelism, Environment.ProcessorCount - 1)
        };
}

