namespace KadenaNodeWatcher.Core.Models.DbModels;

public class NodeDbModel
{
    public int Id { get; set; }

    public string IpAddress { get; set; }
        
    public string Hostname { get; set; }
        
    public int? Port { get; set; }
        
    public bool? IsOnline { get; set; }
        
    public string NodeVersion { get; set; }
        
    public DateTime Timestamp { get; set; }
}