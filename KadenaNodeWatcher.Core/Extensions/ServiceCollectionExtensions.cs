using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.DbConnection;
using KadenaNodeWatcher.Core.Logs;
using Microsoft.Extensions.DependencyInjection;

namespace KadenaNodeWatcher.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterCore(this IServiceCollection services)
    {
        services.AddTransient<IDbConnectionFactory, NodeDbConnectionFactory>();
        services.AddTransient<IDbLogger, DbLogger>();
    }
}