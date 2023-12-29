using CommandLine;
using KadenaNodeWatcher.Collector;
using KadenaNodeWatcher.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    
    services.RegisterCore(configuration, "App");
    services.AddTransient<App>();
}

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