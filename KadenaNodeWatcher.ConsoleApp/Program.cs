using CommandLine;
using IpGeolocation.Extensions;
using KadenaNodeWatcher.ConsoleApp;
using KadenaNodeWatcher.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

var services = new ServiceCollection();

ConfigureServices();

RunningOptions runningOptions = CheckCommandLineOptions(args); 

// create service provider
var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetService<App>()!.Run(runningOptions);

serviceProvider.Dispose();

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
    
    services.UseIpGeolocation(configuration);
    
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

static RunningOptions CheckCommandLineOptions(string[] args)
{
    RunningOptions options = new RunningOptions();
    Parser.Default.ParseArguments<RunningOptions>(args)
        .WithParsed(opt =>
        {
            options = opt;
        })
        .WithNotParsed(_ =>
        {
            // in case of parameter parsing errors or using help option close the application
            Environment.Exit(0);
        });
    return options;
}