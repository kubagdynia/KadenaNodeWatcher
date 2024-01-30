using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/numberOfNodes/{date:datetime}", async //Task<Results<Ok<int>, BadRequest<string>>>
        (DateTime date, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
        {
            int numberOfNodes = await kadenaNodeWatcherService.GetNumberOfNodes(date);
            return TypedResults.Ok(numberOfNodes);
        })
    .WithName("GetNumberOfNodes")
    .WithOpenApi();

app.MapGet("/nodes",
        async (DateTime? date, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
        {
            date ??= DateTime.Now;
            
            IEnumerable<NodeDataDto> nodes = await kadenaNodeWatcherService.GetNodes(date.Value);
            return TypedResults.Ok(nodes);
        })
    .WithName("GetNodes")
    .WithOpenApi();

app.Run();