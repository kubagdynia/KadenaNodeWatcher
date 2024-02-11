using IpGeolocation.Models;
using IpGeolocation.Services;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Logs.Models;
using KadenaNodeWatcher.Core.Models;
using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Repositories;
using KadenaNodeWatcher.Core.Repositories.DbModels;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.Core.Services;

internal class KadenaNodeWatcherService(
    IChainwebNodeService chainwebNodeService,
    IIpGeolocationService ipGeolocationService,
    INodeRepository nodeRepository,
    IOptions<ChainwebSettings> chainwebSettings,
    IAppLogger appLogger)
    : IKadenaNodeWatcherService
{
    private readonly ChainwebSettings _chainwebSettings = chainwebSettings.Value;

    public async Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false, CancellationToken ct = default)
    {
        List<Peer> uniquePeers = [];

        GetCutNetworkPeerInfoResponse response = await chainwebNodeService.GetCutNetworkPeerInfoAsync(hostName, ct);
        uniquePeers.AddRange(response.Page.Items);
        
        string ip = GetIp(hostName);
        
        IpGeolocationModel ipGeolocation = null;
        if (checkIpGeolocation)
        {
            IpGeolocationDb ipGeolocationDb = await nodeRepository.GetIpGeolocationAsync(ip);

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
        
        NodeDataResponse nodeDataResponse = new NodeDataResponse
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
        
        int numberOfNodes = await nodeRepository.GetNumberOfNodes(DateTime.Now);
        
        if (numberOfNodes > 0)
        {
            appLogger.AddInfoLog($"Nodes data has already been collected today: {numberOfNodes}.",
                DbLoggerOperationType.GetNodesData);
            await Task.CompletedTask;
            return;
        }
        
        appLogger.AddInfoLog("Start collecting data from root node...", DbLoggerOperationType.GetNodesData);
        
        ConcurrentList<Peer> uniquePeers = [];
        
        GetCutNetworkPeerInfoResponse response = await chainwebNodeService.GetCutNetworkPeerInfoAsync(ct);
        uniquePeers.AddRange(response.Page.Items);

        appLogger.AddInfoLog($"Finish. Unique nodes: {uniquePeers.Count}",
            DbLoggerOperationType.GetNodesData);
        
        // Returns a full or partial list of child nodes
        List<Peer> peers = PreparePeers(response.Page.Items);
        
        // Limiting the maximum degree of parallelism to 3
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = Math.Min(3, Environment.ProcessorCount - 1)
        };

        appLogger.AddInfoLog($"Start collecting node data from child nodes...", DbLoggerOperationType.GetNodesData);
        
        await Parallel.ForEachAsync(peers, parallelOptions, async (peer, _) =>
        {
            await GetUniquePeers(peer, uniquePeers, ct);
        });

        appLogger.AddInfoLog($"Finish. Unique nodes: {uniquePeers.Count}.",
            DbLoggerOperationType.GetNodesData);
        
        AddIpAddress(uniquePeers);
        
        uniquePeers.Sort((peer, peer1) =>
            string.Compare(peer.Address.Hostname, peer1.Address.Hostname, StringComparison.Ordinal));
        
        List<Peer> uniquePeersToCheck = uniquePeers.Where(c => c.IsOnline is null).ToList();

        appLogger.AddInfoLog($"Start checking, whether nodes are online and what is their version...",
            DbLoggerOperationType.GetNodesData);
        
        foreach (var value1 in uniquePeersToCheck)
        {
            value1.IsOnline = true;
        }
        
        // The code will be executed in parallel by up to three threads
        await Parallel.ForEachAsync(uniquePeersToCheck, parallelOptions, async (peer, _) =>
        {
            await IsOnline(peer, ct);
        });

        int online = uniquePeers.Count(c => c.IsOnline.HasValue && c.IsOnline.Value);
        int offline = uniquePeers.Count - online;
        appLogger.AddInfoLog($"Finish. Online: {online}, Offline: {offline}",
            DbLoggerOperationType.GetNodesData);
        
        // Prepare nodes information to be stored in the database 
        List<NodeDbModel> nodeList = uniquePeers.Select(
            peer => NodeDbModel.CreateNodeDbModel(
                peer.Address.Ip,
                peer.Address.Hostname,
                peer.Address.Port,
                peer.IsOnline,
                peer.ChainwebNodeVersion)
            ).ToList();
        
        // Saving nodes info in the db
        await nodeRepository.AddNodes(nodeList);

        if (checkIpGeolocation)
        {
            appLogger.AddInfoLog($"Start checking IP geolocation...",
                DbLoggerOperationType.GetNodesData);
            
            foreach (Peer peer in uniquePeers)
            {
                if (!await nodeRepository.IpGeolocationExistsAsync(peer.Address.Ip))
                {
                    IpGeolocationModel ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(peer.Address.Ip);
                    if (ipGeolocation is not null)
                    {
                        await nodeRepository.AddIpGeolocationAsync(ipGeolocation.ToDbModel());
                    }
                }
            }
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
        IEnumerable<NumberOfNodesGroupedByDatesDb> numberOfNodesGroupdeByDates =
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
        IEnumerable<NumberOfNodesGroupedByCountryDb> nodesGroupedByCountry =
            await nodeRepository.GetNumberOfNodesGroupedByCountry(dateTime, isOnline);

        return nodesGroupedByCountry.Select(
            item => new NumberOfNodesGroupedByCountryDto
            {
                CountryName = item.CountryName,
                CountryCode = item.CountryCode,
                Count = item.Count
            }).ToList();
    }

    public async Task<IEnumerable<FullNodeDataDto>> GetNodes(DateTime date, bool? isOnline = null)
    {
        IEnumerable<FullNodeDataDb> nodes = await nodeRepository.GetNodes(date, isOnline);
        
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
        IEnumerable<NodeDbModel> nodes = await nodeRepository.GetNodesWithoutIpGeolocation(numberOfRecords);
        
        foreach (var node in nodes)
        {
            if (!await nodeRepository.IpGeolocationExistsAsync(node.IpAddress))
            {
                IpGeolocationModel ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(node.IpAddress);
                if (ipGeolocation is not null)
                {
                    await nodeRepository.AddIpGeolocationAsync(ipGeolocation.ToDbModel());
                }
            }
        }
        
        await Task.CompletedTask;
    } 

    private List<Peer> PreparePeers(List<Peer> items)
    {
        NetworkConfig networkConfig = _chainwebSettings.GetSelectedNetworkConfig();
        
        // Sometimes there is a need not to check all the nodes and only a part, which can be selected randomly.
        // We rely on the configuration that was specified in the ChildNodes section.
        
        if (items == null || networkConfig?.ChildNodes == null || networkConfig.ChildNodes.Count >= items.Count)
        {
            return items;
        }

        int numberOfChildNodes = networkConfig.ChildNodes.Count <= 0 ? items.Count : networkConfig.ChildNodes.Count;

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
        List<Peer> peers = uniquePeers.Where(c => c.Address.Hostname == peer.Address.Hostname).ToList();
        
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
    
    private void AddIpAddress(ConcurrentList<Peer> uniquePeers)
    {
        foreach (var peer in uniquePeers)
        {
            if (peer is null)
            {
                continue;
            }

            peer.Address.Ip = GetIp(peer.Address.Hostname);
        }
    }

    private string GetIp(string hostName)
    {
        UriHostNameType uriHostNameType = Uri.CheckHostName(hostName);
        if (uriHostNameType == UriHostNameType.Dns)
        {
            try
            {
                System.Net.IPAddress[] ddIpAddresses = System.Net.Dns.GetHostAddresses(hostName);
                var ipAddress = ddIpAddresses.FirstOrDefault();
                return ipAddress?.ToString();
            }
            catch
            {
                // ignored
            }
        }
        else if (uriHostNameType == UriHostNameType.Unknown)
        {
            try
            {
                Uri uri = new Uri(hostName);
                return uri.GetIp();
            }
            catch
            {
                // ignored
            }

            return hostName;
        }
        else
        {
            return hostName;
        }
        
        return string.Empty;
    }
}

