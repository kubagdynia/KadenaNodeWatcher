using System;
using System.Data;
using System.IO;
using DbConnectionExtensions.DbConnection.Base;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DbConnectionExtensions.DbConnection
{
    public abstract class SqliteDbConnectionFactory : BaseDbConnectionFactory
    {
        private const string ConfigurationSection = "SQLiteConnectionStrings";

        private readonly string _fileName;
        
        private static bool _firstRun = true;
        
        public override string ConnectionName { get; }
        
        protected SqliteDbConnectionFactory(IConfiguration config, string connectionName = "DefaultConnection")
        {
            ConnectionName = connectionName;

            string useConnection = config.GetSection(ConfigurationSection)["UseConnection"] ?? connectionName;

            SqliteConfiguration sqliteConfiguration =
                config.GetSection($"{ConfigurationSection}:{useConnection}").Get<SqliteConfiguration>();

            _fileName = Path.Combine(Environment.CurrentDirectory, sqliteConfiguration.DbFilename);

            InitializeDatabase();
        }
        
        public override IDbConnection Connection() => new SqliteConnection($"DataSource={_fileName}");
        
        protected abstract void CreateDb(IDbConnection dbConnection);

        protected abstract void UpdateDb(IDbConnection dbConnection);
        
        private void InitializeDatabase()
        {
            if (File.Exists(_fileName))
            {
                if (_firstRun)
                {
                    DbUpdate();
                }
                return;
            }

            DbCreate();
        }
        
        private void DbCreate()
        {
            var fileStream = File.Create(_fileName);
            fileStream.Close();

            using var conn = Connection();
            conn.Open();
            try
            {
                CreateDb(conn);
            }
            finally
            {
                conn.Close();
            }
        }
        
        private void DbUpdate()
        {
            using var conn = Connection();
            conn.Open();
            try
            {
                UpdateDb(conn);
            }
            finally
            {
                conn.Close();
            }

            _firstRun = false;
        }
    }
}