using KadenaNodeWatcher.ConsoleApp.Services;

namespace KadenaNodeWatcher.ConsoleApp;

public class App
{
    private readonly IChainwebNodeService _chainwebNodeService;

    public App(IChainwebNodeService chainwebNodeService)
    {
        _chainwebNodeService = chainwebNodeService;
    }
    public async Task Run()
    {
        string address = "https://us-e1.chainweb.com";
        var result = await _chainwebNodeService.GetCutNetworkPeerInfoAsync(address);
        Console.WriteLine(result.ResponseHeaders.ChainwebNodeVersion);

        var getCutResponse = await _chainwebNodeService.GetCutAsync(address);
        Console.WriteLine($"Node version: {getCutResponse.ResponseHeaders.ChainwebNodeVersion}");
        Console.WriteLine($"Node height: {getCutResponse.Cut.Height}");
        
    }
}