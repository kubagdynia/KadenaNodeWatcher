using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Models.NodeData;

namespace KadenaNodeWatcher.Core.Services;

public interface IKadenaNodeWatcherService
{
    Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false, CancellationToken ct = default);

    Task CollectNodeData(bool checkIpGeolocation = false, CancellationToken ct = default);

    Task<int> GetNumberOfNodes(DateTime dateTime, bool? isOnline = null);
    
    Task<IEnumerable<NumberOfNodesGroupedByCountryDto>> GetNumberOfNodesGroupedByCountry(DateTime dateTime, bool? isOnline = null);

    Task<IEnumerable<FullNodeDataDto>> GetNodes(DateTime date, bool? isOnline = null);
}