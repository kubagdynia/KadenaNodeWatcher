namespace KadenaNodeWatcher.Core.Extensions;

public static class UriExtensions
{
    public static string GetIp(this Uri uri)
    {
        if (uri.HostNameType == UriHostNameType.Dns)
        {
            try
            {
                System.Net.IPAddress[] ddIpAddresses = System.Net.Dns.GetHostAddresses(uri.Host);
                var ipAddress = ddIpAddresses.FirstOrDefault();
                return ipAddress?.ToString();
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
}