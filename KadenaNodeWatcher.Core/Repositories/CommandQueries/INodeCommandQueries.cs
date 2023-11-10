namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

public interface INodeCommandQueries
{
    string AddNode { get; }

    string CountNodes(bool? isOnline = null);
}