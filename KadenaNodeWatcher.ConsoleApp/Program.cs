using KadenaNodeWatcher.ConsoleApp;
using KadenaNodeWatcher.ConsoleApp.Services;
using KadenaNodeWatcher.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        });
}
