using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

public interface INodeRepository
{
    Task AddNode(NodeDbModel node);

    Task AddNodes(IEnumerable<NodeDbModel> nodes);

    Task<int> CountNodes(DateTime date, bool? isOnline = null);

    Task<IpGeolocationDb> GetIpGeolocationAsync(string ip);

    Task<bool> IpGeolocationExistsAsync(string ip);

    Task AddIpGeolocationAsync(IpGeolocationDb ipGeolocation);
}