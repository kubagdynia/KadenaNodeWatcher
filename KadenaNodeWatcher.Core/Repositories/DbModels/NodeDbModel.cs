namespace KadenaNodeWatcher.Core.Repositories.DbModels;

internal class NodeDbModel
{
    public int Id { get; set; }

    public string IpAddress { get; set; }
        
    public string Hostname { get; set; }
        
    public int? Port { get; set; }
        
    public bool? IsOnline { get; set; }
        
    public string NodeVersion { get; set; }
    
    public long Created { get; set; }

    public static NodeDbModel CreateNodeDbModel(
        string ipAddress, string hostname, int? port, bool? isOnline, string nodeVersion)
        => new()
        {
            IpAddress = ipAddress,
            Hostname = hostname,
            Port = port,
            IsOnline = isOnline,
            NodeVersion = nodeVersion
        };
}