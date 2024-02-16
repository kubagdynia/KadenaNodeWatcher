using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

internal interface IChainwebNodeService
{
    /// <summary>
    /// Query the current cut from a Chainweb node.
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <param name="ct">Cancellation Token</param>
    Task<GetCutResponse> GetCutAsync(string baseAddress, CancellationToken ct = default);
    
    /// <summary>
    /// Returns an object containing the peers from the peer database of the remote node and base node headers
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Peers from the peer database of the remote node and base node headers</returns>
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress, CancellationToken ct = default);
    
}