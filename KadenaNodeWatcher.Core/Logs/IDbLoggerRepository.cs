using KadenaNodeWatcher.Core.Logs.Models.DbModels;

namespace KadenaNodeWatcher.Core.Logs;

public interface IDbLoggerRepository
{
    Task AddLog(LogDbModel logDbModel);
}