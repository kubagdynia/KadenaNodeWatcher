using KadenaNodeWatcher.Core.Logs.Models;

namespace KadenaNodeWatcher.Core.Logs;

public interface IDbLogger
{
    /// <summary>
    /// Adds an info message to the db log.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    /// <param name="operationType">The type of the performed operation. Default: None</param>
    void AddInfoLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);

    /// <summary>
    /// Adds a warning message to the db log.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    /// <param name="operationType">The type of the performed operation. Default: None</param>
    void AddWarningLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);

    /// <summary>
    /// Adds an error message to the db log.
    /// </summary>
    /// <param name="exception">The message to be added.</param>
    /// <param name="operationType">The type of the performed operation. Default: None</param>
    void AddErrorLog(Exception exception, DbLoggerOperationType operationType = DbLoggerOperationType.None);

    /// <summary>
    /// Adds an error message to the db log.
    /// </summary>
    /// <param name="message">The message to be added.</param>
    /// <param name="operationType">The type of the performed operation. Default: None</param>
    void AddErrorLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None);
}