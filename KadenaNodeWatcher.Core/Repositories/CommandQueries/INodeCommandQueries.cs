namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal interface INodeCommandQueries
{
    string AddNode { get; }

    string GetNumberOfNodes(bool? isOnline = null);
    
    string IpGeolocationExists { get; }
    
    string GetIpGeolocation { get; }
    
    string AddIpGeolocation { get; }
}