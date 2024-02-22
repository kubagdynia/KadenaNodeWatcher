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
                    var ddIpAddresses = System.Net.Dns.GetHostAddresses(hostName);
                    var ipAddress = ddIpAddresses.FirstOrDefault();
                    return ipAddress?.ToString();
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