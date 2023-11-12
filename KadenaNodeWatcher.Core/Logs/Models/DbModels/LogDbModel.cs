namespace KadenaNodeWatcher.Core.Logs.Models.DbModels;

public class LogDbModel
{
    public int Id { get; set; }

    public DateTime Created { get; set; }

    /// <summary>
    /// The type of the performed operation.
    /// </summary>
    public string OperationType { get; set; }

    /// <summary>
    /// Operation status [None, Info, Warning, Error]
    /// </summary>
    public string OperationStatus { get; set; }

    public string Content { get; set; }
}