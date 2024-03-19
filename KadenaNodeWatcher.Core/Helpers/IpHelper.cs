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
                    IPAddress[] ddIpAddresses = Dns.GetHostAddresses(hostName);
                    IPAddress ipAddress = ddIpAddresses.FirstOrDefault();
                    string ipAddr = ipAddress?.ToString();
                    
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
                    Uri uri = new Uri(hostName);
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