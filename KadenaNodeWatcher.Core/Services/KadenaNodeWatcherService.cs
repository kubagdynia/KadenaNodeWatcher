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
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.Core.Services;

public interface IKadenaNodeWatcherService
{
    Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false);

    Task CollectNodeData();
}

public class KadenaNodeWatcherService(
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
            
        Uri uri = new Uri(hostName);
        
        IpGeolocationModel ipGeolocation = null;
        if (checkIpGeolocation)
        {
            ipGeolocation = await ipGeolocationService.GetIpGeolocationAsync(uri.GetIp());
        }
        
        NodeDataResponse nodeDataResponse = new NodeDataResponse()
        {
            HostName = hostName,
            Host = uri.Host,
            Ip = uri.GetIp(),
            ChainwebNodeVersion = response.ResponseHeaders.ChainwebNodeVersion,
            ServerTimestamp = response.ResponseHeaders.ServerTimestamp,
            ServerDateTime = response.ResponseHeaders.ServerTimestamp.UnixTimeToUtcDateTime(),
            UniquePeers = uniquePeers.Count
        };
        if (checkIpGeolocation && ipGeolocation is not null)
        {
            nodeDataResponse.IpGeo = new IpGeo
            {
                CountryName = ipGeolocation.CountryName,
                CountryCodeIso3 = ipGeolocation.CountryCodeIso3,
                City = ipGeolocation.City,
                RegionCode = ipGeolocation.RegionCode,
                Region = ipGeolocation.Region
            };
        }

        return await Task.FromResult(nodeDataResponse);
    }

    public async Task CollectNodeData()
    {
        int count = await nodeRepository.CountNodes(DateTime.Now);
        
        if (count > 0)
        {
            dbLogger.AddInfoLog($"Nodes info has already been collected today: {count}.",
                DbLoggerOperationType.GetNodesInfo);
            await Task.CompletedTask;
        }
        
        dbLogger.AddInfoLog("Data collection from nodes has started.", DbLoggerOperationType.GetNodesInfo);
        
        List<Peer> uniquePeers = [];
        
        GetCutNetworkPeerInfoResponse response = await chainwebNodeService.GetCutNetworkPeerInfoAsync();
        uniquePeers.AddRange(response.Page.Items);
        
        Console.WriteLine($"--------------- {uniquePeers.Count}");
        
        List<Peer> peers = PreparePeers(response.Page.Items);
        
        int peersCount = peers.Count;
        for (int i = 0; i < peersCount; i++)
        {
            await GetUniquePeers(peers[i], uniquePeers);
        }
        
        Console.WriteLine($"END - {uniquePeers.Count}");

        await Task.CompletedTask;
    }
    
    private List<Peer> PreparePeers(List<Peer> items)
    {
        NetworkConfig networkConfig = _appSettings.GetSelectedNetworkConfig();

        if (items == null || networkConfig?.ChildNodes == null || networkConfig.ChildNodes.Count >= items.Count)
        {
            return items;
        }

        int count = networkConfig.ChildNodes.Count <= 0 ? items.Count : networkConfig.ChildNodes.Count;

        if (!networkConfig.ChildNodes.RandomPick)
        {
            return items.Take(count).ToList();
        }

        return items.GetRandomElements(count);
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
    private async Task IsOnline(Peer peer)
    {
        if (peer is null) return;

        try
        {
            var response =
                await chainwebNodeService.GetCutAsync($"https://{peer.Address.Hostname}:{peer.Address.Port}");

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
}

