using KadenaNodeWatcher.Core.Logs.Models;

namespace KadenaNodeWatcher.Core.Logs;

public class AppLogger(IDbLogger dbLogger) : IAppLogger
{
    public void AddInfoLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddInfoLog(message, operationType);
    }

    public void AddWarningLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddWarningLog(message, operationType);
    }

    public void AddErrorLog(Exception exception, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        dbLogger.AddErrorLog(exception, operationType);
    }

    public void AddErrorLog(string message, DbLoggerOperationType operationType = DbLoggerOperationType.None)
    {
        Console.WriteLine(message);
        dbLogger.AddErrorLog(message, operationType);
    }
}