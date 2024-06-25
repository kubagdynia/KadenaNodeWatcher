using DbConnectionExtensions.DbConnection.Base;
using IpGeolocation.Extensions;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.DbConnection;
using KadenaNodeWatcher.Core.Logs;
using KadenaNodeWatcher.Core.Repositories;
using KadenaNodeWatcher.Core.Repositories.CommandQueries;
using KadenaNodeWatcher.Core.Services;
using KadenaNodeWatcher.Core.Statistics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace KadenaNodeWatcher.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterCore(this IServiceCollection services, IConfiguration configuration, string chainwebConfigSectionName = "Chainweb")
    {
        var chainwebConfig = configuration.GetSection(chainwebConfigSectionName);

        if (!chainwebConfig.Exists())
        {
            // Create default configuration
            chainwebConfig = DefaultChainwebConfig.GetDefaultChainwebConfig(chainwebConfigSectionName);
        }
        
        services.Configure<ChainwebSettings>(chainwebConfig);
        services.AddSingleton(configuration);
        
        services.AddSingleton<IChainwebCommon, ChainwebCommon>();
        services.AddSingleton<IChainwebNodeService, ChainwebNodeService>();
        
        services.AddTransient<IDbConnectionFactory, NodeDbConnectionFactory>();
        services.AddSingleton<INodeCommandQueries, NodeCommandQueries>();

        services.AddTransient<INodeRepository, NodeRepository>();
        
        services.AddTransient<IDbLoggerRepository, DbLoggerRepository>();
        services.AddTransient<IDbLogger, DbLogger>();
        services.AddTransient<IAppLogger, AppLogger>();
        
        services.AddTransient<IStatsRepository, StatsRepository>();
        services.AddTransient<IStats, Stats>();

        services.AddTransient<IKadenaNodeWatcherService, KadenaNodeWatcherService>();
        
        services.UseIpGeolocation(configuration);
        
        AddHttpClient(services);
    }
    
    private static void AddHttpClient(IServiceCollection services)
    {
        services.AddHttpClient("ClientWithoutSSLValidation")
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    // Disable SSL certificate validation
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
            })
            .AddPolicyHandler(GetRetryPolicy(1))
            .AddPolicyHandler(GetTimeoutPolicy(1));
    
        services.AddHttpClient("HttpClient")
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int seconds = 5)
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(seconds));

    static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(int seconds = 5)
        => Policy.TimeoutAsync<HttpResponseMessage>(seconds,
            TimeoutStrategy.Optimistic, onTimeoutAsync: (_, _, _, _) => Task.CompletedTask);
}