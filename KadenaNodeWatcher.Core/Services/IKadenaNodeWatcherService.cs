using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Models.NodeData;

namespace KadenaNodeWatcher.Core.Services;

public interface IKadenaNodeWatcherService
{
    Task<NodeDataResponse> GetNodeData(string hostName, bool checkIpGeolocation = false, CancellationToken ct = default);

    Task CollectNodeData(CancellationToken ct = default);

    Task<int> GetNumberOfNodes(DateTime dateTime, bool? isOnline = null);

    Task<IEnumerable<NumberOfNodesGroupedByDatesDto>> GetNumberOfNodesGroupedByDates(DateTime dateFrom, DateTime dateTo);
    
    Task<IEnumerable<NumberOfNodesGroupedByCountryDto>> GetNumberOfNodesGroupedByCountry(DateTime dateTime, bool? isOnline = null, string nodeVersion = null);

    Task<IEnumerable<NumberOfNodesGroupedByVersionDto>> GetNumberOfNodesGroupedByVersion(DateTime dateTime, bool? isOnline = null);

    Task<IEnumerable<FullNodeDataDto>> GetNodes(DateTime date, bool? isOnline = null);

    Task CollectNodeIpGeolocations(int numberOfRecords);
}