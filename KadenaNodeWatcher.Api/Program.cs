using KadenaNodeWatcher.Api;
using KadenaNodeWatcher.Core.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, false)
    .AddCommandLine(args)
    .AddEnvironmentVariables();

// get connection string
var redisConnectionString = builder.Configuration.GetConnectionString("RedisCache");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add health checks
builder.Services.AddHealthChecks();

// redis configuration for output cache
builder.Services.AddStackExchangeRedisOutputCache(opt => opt.Configuration = redisConnectionString);
builder.Services.AddOutputCache(opt =>
{
    opt.AddBasePolicy(c => c.Expire(TimeSpan.FromSeconds(60)));
    opt.AddPolicy("CustomPolicy", c => c.Expire(TimeSpan.FromSeconds(30)));
});

// defining Serilog configs
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// configure logging
builder.Services.AddLogging(b =>
{ 
    b.AddSerilog();
});

builder.Services.AddProblemDetails();

builder.Services.RegisterCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseOutputCache();

app.UseHttpsRedirection();

// Health checks
app.UseHealthChecks("/kadenanodes/api/health");

// Map the NodeWatcher endpoints
app.MapNodeWatcherEndpoints();

app.Run();