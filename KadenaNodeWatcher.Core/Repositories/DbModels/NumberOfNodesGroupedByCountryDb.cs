namespace KadenaNodeWatcher.Core.Repositories.DbModels;

public class NumberOfNodesGroupedByCountryDb
{
    public string CountryName { get; set; }
    
    public string CountryCode { get; set; }
    
    public int Count { get; set; }
}