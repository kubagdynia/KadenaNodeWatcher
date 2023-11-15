using DbConnectionExtensions.DbConnection.Base;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.DbConnection;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Repositories.CommandQueries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KadenaNodeWatcher.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterCore(this IServiceCollection services, IConfiguration configuration, string configurationSectionName = null)
    {
        if (configuration is not null)
        {
            if (!string.IsNullOrEmpty(configurationSectionName))
            {
                services.Configure<AppSettings>(configuration.GetSection(configurationSectionName));
            }

            services.AddSingleton(configuration);
        }
        
        services.AddSingleton<IChainwebCommon, ChainwebCommon>();
        services.AddSingleton<IChainwebNodeService, ChainwebNodeService>();
        
        services.AddTransient<IDbConnectionFactory, NodeDbConnectionFactory>();
        services.AddSingleton<INodeCommandQueries, NodeCommandQueries>();
        
        services.AddTransient<IDbLoggerRepository, DbLoggerRepository>();
        services.AddTransient<IDbLogger, DbLogger>();
    }
}