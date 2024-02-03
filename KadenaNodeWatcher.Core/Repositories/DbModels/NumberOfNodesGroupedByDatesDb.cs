namespace KadenaNodeWatcher.Core.Repositories.DbModels;

public class NumberOfNodesGroupedByDatesDb
{
    public long Date { get; set; }

    public int TotalCount { get; set; }

    public int Online { get; set; }
    
    public int Offline { get; set; }
}