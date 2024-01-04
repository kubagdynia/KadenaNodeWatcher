namespace KadenaNodeWatcher.Core.Repositories.DbModels;

internal class IpGeolocationDb
{
    public int Id { get; set; }
    
    public string IpAddress { get; set; }
    
    public string City { get; set; }
    
    public string Country { get; set; }
    
    public string CountryCode { get; set; }
    
    public string CountryCodeIso3 { get; set; }
    
    public string CountryName { get; set; }
    
    public string ContinentCode { get; set; }
    
    public string RegionCode { get; set; }
    
    public string Region { get; set; }
    
    public string Org { get; set; }
}