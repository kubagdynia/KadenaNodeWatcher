using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterCore(builder.Configuration, "App");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/numberOfNodes/{date:datetime}",
        async (DateTime date, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
        {
            int numberOfNodes = await kadenaNodeWatcherService.GetNumberOfNodes(date);
            return numberOfNodes;

        })
    .WithName("GetNumberOfNodes")
    .WithOpenApi();

app.Run();