using KadenaNodeWatcher.Core.Logs.Models;

namespace KadenaNodeWatcher.Core.Logs;

public interface IAppLogger
{
    void AddInfoLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);
    void AddWarningLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);
    void AddErrorLog(Exception exception, DbLoggerOperationType operationType = DbLoggerOperationType.None);
    void AddErrorLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);
}