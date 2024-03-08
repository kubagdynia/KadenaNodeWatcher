namespace KadenaNodeWatcher.Core.Repositories.DbModels;

internal class NumberOfNodesGroupedByVersionDb
{
    public string NodeVersion { get; set; }
    
    public long Date { get; set; }
    
    public int Count { get; set; }
}