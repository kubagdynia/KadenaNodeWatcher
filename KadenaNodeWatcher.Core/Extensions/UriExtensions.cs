using System.Net;
using KadenaNodeWatcher.Core.Helpers;
using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Extensions;

internal static class UriExtensions
{
    internal static string GetIp(this Uri uri)
    {
        if (uri.HostNameType == UriHostNameType.Dns)
        {
            try
            {
                IPAddress[] ddIpAddresses = Dns.GetHostAddresses(uri.Host);
                IPAddress ipAddress = ddIpAddresses.FirstOrDefault();
                string ipAddr = ipAddress?.ToString();
                
                return string.IsNullOrEmpty(ipAddr) ? null : ipAddr;
            }
            catch
            {
                // ignored
            }
        }
        else
        {
            return uri.Host;
        }
        
        return string.Empty;
    }
    
    internal static void AddIpAddress(this ConcurrentList<Peer> peers)
    {
        foreach (var peer in peers)
        {
            if (peer is null)
            {
                continue;
            }

            peer.Address.Ip = IpHelper.GetIp(peer.Address.Hostname);
        }
    }
}