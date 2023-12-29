using IpGeolocation.Models;
using IpGeolocation.Services;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Models;
using KadenaNodeWatcher.Core.Models.NodeData;

namespace KadenaNodeWatcher.Core.Services;

public interface IKadenaNodeWatcherService
{
    Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false);
}

public class KadenaNodeWatcherService(
    IChainwebNodeService chainwebNodeService, IIpGeolocationService ipGeolocationService) : IKadenaNodeWatcherService
{
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
}

