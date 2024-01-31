using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

internal interface INodeRepository
{
    Task AddNode(NodeDbModel node);

    Task AddNodes(IEnumerable<NodeDbModel> nodes);

    Task<int> GetNumberOfNodes(DateTime date, bool? isOnline = null);

    Task<IEnumerable<FullNodeDataDb>> GetNodes(DateTime date);

    Task<IpGeolocationDb> GetIpGeolocationAsync(string ip);

    Task<bool> IpGeolocationExistsAsync(string ip);

    Task AddIpGeolocationAsync(IpGeolocationDb ipGeolocation);
}