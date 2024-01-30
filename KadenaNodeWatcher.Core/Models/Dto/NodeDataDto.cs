namespace KadenaNodeWatcher.Core.Models.Dto;

public class NodeDataDto
{
    public int Id { get; set; }

    public string IpAddress { get; set; }
        
    public string Hostname { get; set; }
        
    public int? Port { get; set; }
        
    public bool? IsOnline { get; set; }
        
    public string NodeVersion { get; set; }

    public static NodeDataDto Create(
        int id, string ipAddress, string hostname, int? port, bool? isOnline, string nodeVersion)
        => new()
        {
            Id = id,
            IpAddress = ipAddress,
            Hostname = hostname,
            Port = port,
            IsOnline = isOnline,
            NodeVersion = nodeVersion
        };
}