namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal interface INodeCommandQueries
{
    string AddNode { get; }

    string GetNumberOfNodes(bool? isOnline = null);
    
    string GetNumberOfNodesGroupedByDates();

    string GetNumberOfNodesGroupedByCountry(bool? isOnline = null);
    
    string GetNodes(bool? isOnline = null);
    
    string IpGeolocationExists { get; }
    
    string GetIpGeolocation { get; }
    
    string AddIpGeolocation { get; }

    string GetNodesWithoutIpGeolocation(int numberOfRecords);
}