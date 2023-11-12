using KadenaNodeWatcher.ConsoleApp;
using KadenaNodeWatcher.ConsoleApp.Services;
using KadenaNodeWatcher.Core.Chainweb;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

ConfigureServices();

// create service provider
var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetService<App>()!.Run();

void ConfigureServices()
{
    AddHttpClient();
    services.AddTransient<App>();
    services.AddSingleton<IChainwebCommon, ChainwebCommon>();
    services.AddTransient<IChainwebNodeService, ChainwebNodeService>();
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
