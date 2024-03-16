using KadenaNodeWatcher.Core.Logs.Models;
using Microsoft.Extensions.Logging;

namespace KadenaNodeWatcher.Core.Logs;

public class AppLogger(IDbLogger dbLogger, ILogger<AppLogger> logger) : IAppLogger
{
    public void AddInfoLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddInfoLog(message, operationType);
        logger.LogInformation(message);
    }

    public void AddWarningLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddWarningLog(message, operationType);
        logger.LogWarning(message);
    }

    public void AddErrorLog(Exception exception, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        dbLogger.AddErrorLog(exception, operationType);
    }

    public void AddErrorLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddErrorLog(message, operationType);
        logger.LogError(message);
    }
}