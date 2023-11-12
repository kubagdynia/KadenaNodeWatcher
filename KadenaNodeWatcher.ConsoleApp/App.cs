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
        var result = await _chainwebNodeService.GetCutNetworkPeerInfoAsync("https://us-e1.chainweb.com");
        Console.WriteLine(result.ResponseHeaders.ChainwebNodeVersion);
    }
}