using System.Data;
using DbConnectionExtensions.DbConnection;
using Microsoft.Extensions.Configuration;

namespace KadenaNodeWatcher.Core.DbConnection;

public class NodeDbConnectionFactory : SqliteDbConnectionFactory
{
    public NodeDbConnectionFactory(IConfiguration config, string connectionName = "DefaultConnection")
        : base(config, connectionName)
    {
        
    }

    protected override void CreateDb(IDbConnection dbConnection)
    {
        throw new NotImplementedException();
    }

    protected override void UpdateDb(IDbConnection dbConnection)
    {
        throw new NotImplementedException();
    }
}