namespace KadenaNodeWatcher.Core.Repositories.DbModels;

internal class FullNodeDataDb
{
    public int Id { get; set; }
    
    public long Created { get; set; }

    public string IpAddress { get; set; }
        
    public string Hostname { get; set; }
        
    public int? Port { get; set; }
        
    public bool? IsOnline { get; set; }
        
    public string NodeVersion { get; set; }

    public string CountryName { get; set; }
    
    public string CountryCode { get; set; }
    
    public string City { get; set; }
    
    public string ContinentCode { get; set; }
    
    public string Org { get; set; }
}