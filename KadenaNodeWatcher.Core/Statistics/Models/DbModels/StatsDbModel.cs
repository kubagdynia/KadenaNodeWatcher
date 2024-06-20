namespace KadenaNodeWatcher.Core.Statistics.Models.DbModels;

public class StatsDbModel
{
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the operation.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The content of the operation may be null
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// Update date.
    /// </summary>
    public DateTime Updated { get; set; }
}