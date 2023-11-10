using System.Data;
using Dapper;
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
        dbConnection.Execute(
            @"
                CREATE TABLE IF NOT EXISTS Nodes
                    (
	                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
	                    IpAddress							VARCHAR(255) NULL,
	                    Hostname							VARCHAR(255) NULL,
	                    Port                                INTEGER NULL,
                        IsOnline                            BOOLEAN NULL,
                        NodeVersion                         VARCHAR(10) NULL,
                        Created 							DATE DEFAULT (strftime('%s', date('now')))
                    );
                CREATE INDEX Nodes_Created_ix ON Nodes(Created DESC);

                CREATE TABLE IF NOT EXISTS Logs
                    (
	                    Id                                  INTEGER PRIMARY KEY AUTOINCREMENT,
	                    OperationType						VARCHAR(100) NULL,
	                    OperationStatus						VARCHAR(20),
                        Content                             TEXT,
                        Timestamp 							DATE DEFAULT (strftime('%s', 'now'))
                    );
                CREATE INDEX IF NOT EXISTS Logs_Timestamp_ix ON Logs(Timestamp DESC);

                ");
        
        // select datetime(created, 'unixepoch'), date(created, 'unixepoch'),  time(created, 'unixepoch') from Nodes
        // https://tableplus.com/blog/2018/07/sqlite-how-to-use-datetime-value.html
        // DATE DEFAULT (strftime('%s', date('now')))
    }

    protected override void UpdateDb(IDbConnection dbConnection)
    {
        
    }
}