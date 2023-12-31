using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

internal interface IChainwebNodeService
{
    Task<GetCutResponse> GetCutAsync(string baseAddress, CancellationToken ct = default);
    
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync();
    
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress);
    
}