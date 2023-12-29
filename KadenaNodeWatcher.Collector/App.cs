using System.Text.Json;
using System.Text.Json.Serialization;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Models.NodeData;
using KadenaNodeWatcher.Core.Services;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.Collector;

public class App(
    IOptions<AppSettings> appSettings,
    IKadenaNodeWatcherService kadenaNodeWatcherService)
{
    public async Task Run(RunningOptions runningOptions)
    {
        if (!string.IsNullOrEmpty(runningOptions.HostName))
        {
            NodeDataResponse nodeDataResult =
                await kadenaNodeWatcherService.GetNodeData(runningOptions.HostName, checkIpGeolocation: true);

            Console.WriteLine(JsonSerializer.Serialize(nodeDataResult,
                new JsonSerializerOptions
                    { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
        }
    }
}