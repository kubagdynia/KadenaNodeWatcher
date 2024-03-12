using System.Text.Json;
using System.Text.Json.Serialization;
using KadenaNodeWatcher.Core.Services;

namespace KadenaNodeWatcher.Collector;

public class App(IKadenaNodeWatcherService kadenaNodeWatcherService)
{
    public async Task Run(RunningOptions runningOptions)
    {
        using var cts = new CancellationTokenSource();

        if (runningOptions.QueryDatabase)
        {
            var date = DateTime.Now;
            // date format e.g. "2024-01-17"
            if (!string.IsNullOrEmpty(runningOptions.Date))
            {
                date = DateTime.Parse(runningOptions.Date);
            }
            
            var numberOfNodes = await kadenaNodeWatcherService.GetNumberOfNodes(date);
            Console.WriteLine($"Number of nodes for {date:s} is {numberOfNodes}");
        }
        else if (runningOptions.CollectNodeDataAutomatically)
        {
            await kadenaNodeWatcherService.CollectNodeData(cts.Token);

            if (runningOptions.CollectNodeIpGeolocations)
            {
                await kadenaNodeWatcherService.CollectNodeIpGeolocations(runningOptions.NumberOfItems);
            }
        }
        else if (!string.IsNullOrEmpty(runningOptions.HostName))
        {
            var nodeDataResult =
                await kadenaNodeWatcherService.GetNodeData(runningOptions.HostName, runningOptions.CheckIpGeolocation, cts.Token);

            Console.WriteLine(JsonSerializer.Serialize(nodeDataResult, JsonSerializerOptions));
        }
        else if (runningOptions.CollectNodeIpGeolocations)
        {
            await kadenaNodeWatcherService.CollectNodeIpGeolocations(runningOptions.NumberOfItems);
        }
    }
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
}