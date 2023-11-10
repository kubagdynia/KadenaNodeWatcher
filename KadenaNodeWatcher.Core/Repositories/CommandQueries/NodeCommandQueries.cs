namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

public class NodeCommandQueries : INodeCommandQueries
{
    public string AddNode
        => @"INSERT INTO Nodes (IpAddress, Hostname, Port, IsOnline, NodeVersion) VALUES (@IpAddress, @Hostname, @Port, @IsOnline, @NodeVersion)";
    
    public string CountNodes(bool? isOnline = null)
        => isOnline.HasValue
            ? @"SELECT count(*) FROM Nodes WHERE Created = @date AND IsOnline = @isOnline"
            : @"SELECT count(*) FROM Nodes WHERE Created = @date";
}