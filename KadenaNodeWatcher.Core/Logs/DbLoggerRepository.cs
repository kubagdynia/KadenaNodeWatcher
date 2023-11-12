using Dapper;
using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Logs.Models.DbModels;

namespace KadenaNodeWatcher.Core.Logs;

public class DbLoggerRepository : IDbLoggerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbLoggerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task AddLog(LogDbModel logDbModel)
    {
        using var conn = _connectionFactory.Connection();

        await conn.ExecuteAsync(
            "INSERT INTO Logs (OperationType, OperationStatus, Content) VALUES (@OperationType, @OperationStatus, @Content)",
            logDbModel);

        await Task.CompletedTask;
    }
}