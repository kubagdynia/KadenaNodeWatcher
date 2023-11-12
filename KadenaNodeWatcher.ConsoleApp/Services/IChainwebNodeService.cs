using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.ConsoleApp.Services;

public interface IChainwebNodeService
{
    Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress);
    
}