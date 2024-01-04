namespace KadenaNodeWatcher.Core.Extensions;

internal static class UriExtensions
{
    internal static string GetIp(this Uri uri)
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