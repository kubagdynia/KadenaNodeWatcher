using IpGeolocation.Models;
using IpGeolocation.Services;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Logs.Models;
using KadenaNodeWatcher.Core.Models;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Repositories;
using KadenaNodeWatcher.Core.Repositories.DbModels;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.Core.Services;

public interface IKadenaNodeWatcherService
{
    Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false);

    Task CollectNodeData(bool checkIpGeolocation = false);
}

internal class KadenaNodeWatcherService(
    IChainwebNodeService chainwebNodeService,
    IIpGeolocationService ipGeolocationService,
    INodeRepository nodeRepository,
    IOptions<AppSettings> appSettings,
    IDbLogger dbLogger)
    : IKadenaNodeWatcherService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false)
    {
        List<Peer> uniquePeers = [];

        GetCutNetworkPeerInfoResponse response = await chainwebNodeService.GetCutNetworkPeerInfoAsync(hostName);
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

    public async Task CollectNodeData(bool checkIpGeolocation = false)
    {
        int countNodes = await nodeRepository.CountNodes(DateTime.Now);
        
        if (countNodes > 0)
        {
            dbLogger.AddInfoLog($"Nodes info has already been collected today: {countNodes}.",
                DbLoggerOperationType.GetNodesInfo);
            await Task.CompletedTask;
        }
        
        dbLogger.AddInfoLog("Data collection from nodes has started.", DbLoggerOperationType.GetNodesInfo);
        
        List<Peer> uniquePeers = [];
        
        GetCutNetworkPeerInfoResponse response = await chainwebNodeService.GetCutNetworkPeerInfoAsync();
        uniquePeers.AddRange(response.Page.Items);
        
        Console.WriteLine($"--------------- {uniquePeers.Count}");
        
        // Returns a full or partial list of child nodes
        List<Peer> peers = PreparePeers(response.Page.Items);
        
        int peersCount = peers.Count;
        for (int i = 0; i < peersCount; i++)
        {
            await GetUniquePeers(peers[i], uniquePeers);
        }

        Console.WriteLine("AddIpAddress");
        AddIpAddress(uniquePeers);
        
        Console.WriteLine("Sort");
        uniquePeers.Sort((peer, peer1) =>
            string.Compare(peer.Address.Hostname, peer1.Address.Hostname, StringComparison.Ordinal));
        
        Console.WriteLine("IsOnline");

        List<Peer> uniquePeersToCheck = uniquePeers.Where(c => c.IsOnline is null).ToList();

        // Limiting the maximum degree of parallelism to 3
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = Math.Min(3, Environment.ProcessorCount - 1)
        };
        
        // The code will be executed in parallel by up to three threads
        await Parallel.ForEachAsync(uniquePeersToCheck, parallelOptions, async (peer, ct) =>
        {
            await IsOnline(peer, ct);
            Console.WriteLine($"Online {peer.IsOnline} : {peer.Address.Hostname}");
        });
        
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

        Console.WriteLine($"END - {uniquePeers.Count}");

        await Task.CompletedTask;
    }
    
    private List<Peer> PreparePeers(List<Peer> items)
    {
        NetworkConfig networkConfig = _appSettings.GetSelectedNetworkConfig();
        
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
    
    private async Task GetUniquePeers(Peer peer, List<Peer> uniquePeers)
    {
        List<Peer> peers = uniquePeers.Where(c => c.Address.Hostname == peer.Address.Hostname).ToList();
        
        GetCutNetworkPeerInfoResponse response;
        try
        {
            response =
                await chainwebNodeService.GetCutNetworkPeerInfoAsync(
                    $"https://{peer.Address.Hostname}:{peer.Address.Port}");
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

        Console.WriteLine($"--------------- {uniquePeers.Count}");
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
    
    private void AddIpAddress(List<Peer> uniquePeers)
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

