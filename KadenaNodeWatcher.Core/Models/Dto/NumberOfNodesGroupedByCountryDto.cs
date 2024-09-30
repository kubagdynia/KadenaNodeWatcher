namespace KadenaNodeWatcher.Core.Models.Dto;

public record NumberOfNodesGroupedByCountryDto
{
    public string CountryName { get; set; }
    
    public string CountryCode { get; set; }
    
    public int Count { get; set; }
}