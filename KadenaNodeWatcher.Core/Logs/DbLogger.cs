using KadenaNodeWatcher.Core.Logs.Models;
using KadenaNodeWatcher.Core.Logs.Models.DbModels;

namespace KadenaNodeWatcher.Core.Logs;

public class DbLogger(IDbLoggerRepository repository) : IDbLogger
{
    public void AddInfoLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
        => AddLog(message, operationType, DbLoggerOperationStatus.Info);

    public void AddWarningLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
        => AddLog(message, operationType, DbLoggerOperationStatus.Warning);

    public void AddErrorLog(Exception exception, DbLoggerOperationType operationType = DbLoggerOperationType.None)
        => AddErrorLog(GetExceptionMessage(exception), operationType);

    public void AddErrorLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
        => AddLog(message, operationType, DbLoggerOperationStatus.Error);
    
    private void AddLog(
        string message,
        DbLoggerOperationType operationType,
        DbLoggerOperationStatus operationStatus)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        var logDbModel = new LogDbModel
        {
            OperationType = operationType == DbLoggerOperationType.None ? null: operationType.ToStringCode(),
            OperationStatus = operationStatus.ToStringCodeUpperCase(),
            Content = message
        };

        repository.AddLog(logDbModel);
    }

    private string GetExceptionMessage(Exception ex)
        => $"Message: {ex.Message}; StackTrace: {ex.StackTrace}";
}