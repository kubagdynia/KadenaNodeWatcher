namespace KadenaNodeWatcher.Core.Models.Dto;

public record NumberOfNodesGroupedByDatesDto
{
    public DateTime Date { get; set; }

    public int TotalCount { get; set; }

    public int Online { get; set; }
    
    public int Offline { get; set; }
}