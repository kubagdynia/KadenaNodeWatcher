using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Logs.Models.DbModels;

namespace KadenaNodeWatcher.Core.Logs;

public class DbLoggerRepository(IDbConnectionFactory connectionFactory) : IDbLoggerRepository
{
    public async Task AddLog(LogDbModel logDbModel)
    {
        using var conn = connectionFactory.Connection();

        await conn.ExecuteAsync(
            "INSERT INTO Logs (OperationType, OperationStatus, Content) VALUES (@OperationType, @OperationStatus, @Content)",
            logDbModel);
    }
}