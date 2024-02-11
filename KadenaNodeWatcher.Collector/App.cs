using System.Text.Json;
using System.Text.Json.Serialization;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Services;

namespace KadenaNodeWatcher.Collector;

public class App(IKadenaNodeWatcherService kadenaNodeWatcherService)
{
    public async Task Run(RunningOptions runningOptions)
    {
        using var cts = new CancellationTokenSource();

        if (runningOptions.QueryDatabase)
        {
            DateTime date = DateTime.Now;
            // date format e.g. "2024-01-17"
            if (!string.IsNullOrEmpty(runningOptions.Date))
            {
                date = DateTime.Parse(runningOptions.Date);
            }
            int numberOfNodes = await kadenaNodeWatcherService.GetNumberOfNodes(date);
            Console.WriteLine($"Number of nodes for {date:s} is {numberOfNodes}");
        }
        else if (runningOptions.CollectNodeDataAutomatically)
        {
            await kadenaNodeWatcherService.CollectNodeData(checkIpGeolocation: true, cts.Token);
        }
        else if (!string.IsNullOrEmpty(runningOptions.HostName))
        {
            NodeDataResponse nodeDataResult =
                await kadenaNodeWatcherService.GetNodeData(runningOptions.HostName, runningOptions.CheckIpGeolocation, cts.Token);

            Console.WriteLine(JsonSerializer.Serialize(nodeDataResult, JsonSerializerOptions));
        }
        else if (runningOptions.CollectNodeIpGeolocations)
        {
            Console.WriteLine("CollectNodeIpGeolocations: Start");
            await kadenaNodeWatcherService.CollectNodeIpGeolocations(runningOptions.NumberOfItems);
            Console.WriteLine("CollectNodeIpGeolocations: End");
        }
    }
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
}