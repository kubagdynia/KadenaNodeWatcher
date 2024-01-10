using System.Text.Json;
using System.Text.Json.Serialization;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Services;

namespace KadenaNodeWatcher.Collector;

public class App(IKadenaNodeWatcherService kadenaNodeWatcherService)
{
    public async Task Run(RunningOptions runningOptions)
    {
        if (runningOptions.CollectNodeDataAutomatically)
        {
            await kadenaNodeWatcherService.CollectNodeData(checkIpGeolocation: true);
        }
        else if (!string.IsNullOrEmpty(runningOptions.HostName))
        {
            NodeDataResponse nodeDataResult =
                await kadenaNodeWatcherService.GetNodeData(runningOptions.HostName, runningOptions.CheckIpGeolocation);

            Console.WriteLine(JsonSerializer.Serialize(nodeDataResult, JsonSerializerOptions));
        }
    }
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
}