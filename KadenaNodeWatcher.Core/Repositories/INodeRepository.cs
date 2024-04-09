using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

internal interface INodeRepository
{
    Task AddNode(NodeDbModel node);

    Task AddNodes(IEnumerable<NodeDbModel> nodes);

    Task<int> GetNumberOfNodes(DateTime date, bool? isOnline = null);

    Task<IEnumerable<NumberOfNodesGroupedByDatesDb>> GetNumberOfNodesGroupedByDates(DateTime dateFrom, DateTime dateTo);
    
    Task<IEnumerable<NumberOfNodesGroupedByCountryDb>> GetNumberOfNodesGroupedByCountry(DateTime date, bool? isOnline = null, string nodeVersion = null);

    Task<IEnumerable<NumberOfNodesGroupedByVersionDb>> GetNumberOfNodesGroupedByVersion(DateTime date, bool? isOnline = null);

    Task<IEnumerable<FullNodeDataDb>> GetNodes(DateTime date, bool? isOnline = null);

    Task<IpGeolocationDb> GetIpGeolocationAsync(string ip);

    Task<bool> IpGeolocationExistsAsync(string ip);

    Task AddIpGeolocationAsync(IpGeolocationDb ipGeolocation);

    Task<IEnumerable<NodeDbModel>> GetNodesWithoutIpGeolocation(int numberOfRecords);
}