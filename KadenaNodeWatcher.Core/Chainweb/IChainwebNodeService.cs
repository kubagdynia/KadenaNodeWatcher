using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.Core.Chainweb;

public interface IChainwebNodeService
{
    Task<GetCutResponse> GetCutAsync(string baseAddress);
    
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync();
    
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress);
    
}