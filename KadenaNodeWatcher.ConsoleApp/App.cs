using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Logs.Models;

namespace KadenaNodeWatcher.ConsoleApp;

public class App
{
    private readonly IChainwebNodeService _chainwebNodeService;
    private readonly IDbLogger _dbLogger;

    public App(
        IChainwebNodeService chainwebNodeService,
        IDbLogger dbLogger)
    {
        _chainwebNodeService = chainwebNodeService;
        _dbLogger = dbLogger;
    }
    public async Task Run()
    {
        _dbLogger.AddInfoLog("Job started.", DbLoggerOperationType.GetNodesInfo);
        
        string address = "https://us-e1.chainweb.com";
        var result = await _chainwebNodeService.GetCutNetworkPeerInfoAsync(address);
        Console.WriteLine(result.ResponseHeaders.ChainwebNodeVersion);

        var getCutResponse = await _chainwebNodeService.GetCutAsync(address);
        Console.WriteLine($"Node version: {getCutResponse.ResponseHeaders.ChainwebNodeVersion}");
        Console.WriteLine($"Node height: {getCutResponse.Cut.Height}");
        
    }
}