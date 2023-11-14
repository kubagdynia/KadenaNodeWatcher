using KadenaNodeWatcher.ConsoleApp;
using KadenaNodeWatcher.ConsoleApp.Services;
using KadenaNodeWatcher.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

var services = new ServiceCollection();

ConfigureServices();

// create service provider
var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetService<App>()!.Run();

void ConfigureServices()
{
    // build config
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    
    AddHttpClient();
    
    services.RegisterCore(configuration, "App");
    
    services.AddTransient<IChainwebNodeService, ChainwebNodeService>();
    
    services.AddTransient<App>();
}

void AddHttpClient()
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
