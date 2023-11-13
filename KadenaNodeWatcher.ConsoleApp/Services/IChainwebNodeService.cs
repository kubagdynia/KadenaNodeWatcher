using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.ConsoleApp.Services;

public interface IChainwebNodeService
{
    Task<GetCutResponse> GetCutAsync(string baseAddress);
    
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress);
    
}