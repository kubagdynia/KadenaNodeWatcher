using System.Data;

namespace DbConnectionExtensions.DbConnection.Base
{
    public interface IDbConnectionFactory
    {
        IDbConnection Connection();

        IDbConnection Connection(string name);
    }
}