using KadenaNodeWatcher.Core.Models.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

public interface INodeRepository
{
    Task AddNode(NodeDbModel node);

    Task AddNodes(IEnumerable<NodeDbModel> nodes);

    Task<int> CountNodes(DateTime date, bool? isOnline = null);
}