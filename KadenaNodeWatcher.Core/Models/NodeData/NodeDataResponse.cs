namespace KadenaNodeWatcher.Core.Models.NodeData;

public class NodeDataResponse
{
    public string HostName { get; set; }
    
    public string Ip { get; set; }
    
    public string ChainwebNodeVersion { get; set; }
    
    public int ServerTimestamp { get; set; }
    
    public DateTime ServerDateTime { get; set; }
    
    public int UniquePeers { get; set; }
    
    public IpGeo IpGeo { get; set; }
}