using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

public class ChainwebCommon : IChainwebCommon
{
    public ChainwebResponseHeaders GetResponseHeaders(HttpResponseMessage response)
    {
        var headers = new ChainwebResponseHeaders();

        if (response.Headers.TryGetValues("x-chainweb-node-version", out IEnumerable<string> nodeVersion))
        {
            headers.ChainwebNodeVersion = nodeVersion.FirstOrDefault();
        }

        if (response.Headers.TryGetValues("x-peer-addr", out IEnumerable<string> peerAddr))
        {
            headers.PeerAddr = peerAddr.FirstOrDefault();
        }

        if (response.Headers.TryGetValues("x-server-timestamp", out IEnumerable<string> serverTimestamp))
        {
            var value = serverTimestamp.FirstOrDefault();

            if (value != null)
            {
                if (int.TryParse(value, out int number))
                {
                    headers.ServerTimestamp = number;
                }
            }
        }

        return headers;
    }
}