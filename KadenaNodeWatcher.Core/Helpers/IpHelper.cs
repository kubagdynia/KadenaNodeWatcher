using System.Net;
using KadenaNodeWatcher.Core.Extensions;

namespace KadenaNodeWatcher.Core.Helpers;

public static class IpHelper
{
    public static string GetIp(string hostName)
    {
        var uriHostNameType = Uri.CheckHostName(hostName);
        switch (uriHostNameType)
        {
            case UriHostNameType.Dns:
                try
                {
                    var ddIpAddresses = Dns.GetHostAddresses(hostName);
                    var ipAddress = ddIpAddresses.FirstOrDefault();
                    var ipAddr = ipAddress?.ToString();
                    
                    return string.IsNullOrEmpty(ipAddr) ? null : ipAddr;
                }
                catch
                {
                    // ignored
                }

                break;
            case UriHostNameType.Unknown:
                try
                {
                    var uri = new Uri(hostName);
                    return uri.GetIp();
                }
                catch
                {
                    // ignored
                }

                return hostName;
            default:
                return hostName;
        }
        
        return string.Empty;
    }
}