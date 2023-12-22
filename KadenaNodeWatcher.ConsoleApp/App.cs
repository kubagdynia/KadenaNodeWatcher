using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Logs.Models;
using KadenaNodeWatcher.Core.Models;
using KadenaNodeWatcher.Core.Repositories;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.ConsoleApp;

public class App
{
    private readonly IChainwebNodeService _chainwebNodeService;
    private readonly INodeRepository _nodeRepository;
    private readonly IDbLogger _dbLogger;
    private readonly AppSettings _appSettings;

    public App(
        IChainwebNodeService chainwebNodeService,
        INodeRepository nodeRepository,
        IDbLogger dbLogger,
        IOptions<AppSettings> appSettings)
    {
        _chainwebNodeService = chainwebNodeService;
        _nodeRepository = nodeRepository;
        _dbLogger = dbLogger;
        _appSettings = appSettings?.Value;
    }
    
    public async Task Run()
    {
        int count = await _nodeRepository.CountNodes(DateTime.Now);
        
        if (count > 0)
        {
            _dbLogger.AddInfoLog($"Nodes info has already been collected today: {count}.",
                DbLoggerOperationType.GetNodesInfo);
            return;
        }
        
        _dbLogger.AddInfoLog("Job started.", DbLoggerOperationType.GetNodesInfo);
        
        List<Peer> uniquePeers = [];
        
        GetCutNetworkPeerInfoResponse response = await _chainwebNodeService.GetCutNetworkPeerInfoAsync();
        uniquePeers.AddRange(response.Page.Items);
        
        Console.WriteLine($"--------------- {uniquePeers.Count}");
        
        List<Peer> peers = PreparePeers(response.Page.Items);

        int peersCount = peers.Count;
        for (int i = 0; i < peersCount; i++)
        {
            await GetUniquePeers(peers[i], uniquePeers);
        }
        
        Console.WriteLine($"END - {uniquePeers.Count}");
    }
    
    private List<Peer> PreparePeers(List<Peer> items)
    {
        NetworkConfig networkConfig = _appSettings.GetSelectedNetworkConfig();

        if (items == null || networkConfig?.ChildNodes == null
                          || networkConfig.ChildNodes.Count >= items.Count)
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
                await _chainwebNodeService.GetCutNetworkPeerInfoAsync(
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
                await _chainwebNodeService.GetCutAsync($"https://{peer.Address.Hostname}:{peer.Address.Port}");

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