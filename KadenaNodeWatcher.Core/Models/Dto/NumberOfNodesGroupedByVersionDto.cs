namespace KadenaNodeWatcher.Core.Models.Dto;

public class NumberOfNodesGroupedByVersionDto
{
    public string NodeVersion { get; set; }
    
    public DateTime Date { get; set; }
    
    public int Count { get; set; }
}