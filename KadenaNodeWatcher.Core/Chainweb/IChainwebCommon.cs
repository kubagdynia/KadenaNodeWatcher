using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

internal interface IChainwebCommon
{
    ChainwebResponseHeaders GetResponseHeaders(HttpResponseMessage response);
}