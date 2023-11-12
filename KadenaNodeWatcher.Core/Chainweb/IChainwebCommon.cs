using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

public interface IChainwebCommon
{
    ChainwebResponseHeaders GetResponseHeaders(HttpResponseMessage response);
}