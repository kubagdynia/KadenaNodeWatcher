using KadenaNodeWatcher.Core.Extensions;
using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

var nodesEndpoints = app.MapGroup("/api");

nodesEndpoints.MapGet("/nodes", async Task<Results<Ok<IEnumerable<FullNodeDataDto>>, NotFound, BadRequest>>
        (DateTime? date, bool? isOnline, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
    {
        date ??= DateTime.Now;

        IEnumerable<FullNodeDataDto> nodes = (await kadenaNodeWatcherService.GetNodes(date.Value, isOnline)).ToList();
        return !nodes.Any() ? TypedResults.NotFound() : TypedResults.Ok(nodes);
    })
    .WithName("GetNodes")
    .WithSummary("Get full nodes data for a specific date.")
    .WithOpenApi();

nodesEndpoints.MapGet("/nodes/count", async Task<Results<Ok<int>, BadRequest>>
        (DateTime? date, bool? isOnline, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
    {
        date ??= DateTime.Now;

        int numberOfNodes = await kadenaNodeWatcherService.GetNumberOfNodes(date.Value, isOnline);
        return TypedResults.Ok(numberOfNodes);
    })
    .WithName("GetNumberOfNodes")
    .WithSummary("Get number of nodes for a specific date.")
    .WithOpenApi();

nodesEndpoints.MapGet("/nodes/countries/count", async Task<Results<Ok<IEnumerable<NumberOfNodesGroupedByCountryDto>>, NotFound, BadRequest>>
        (DateTime? date, bool? isOnline, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
    {
        date ??= DateTime.Now;

        IEnumerable<NumberOfNodesGroupedByCountryDto> nodesGroupedByCountry =
            (await kadenaNodeWatcherService.GetNumberOfNodesGroupedByCountry(date.Value, isOnline)).ToList();
        
        return !nodesGroupedByCountry.Any() ? TypedResults.NotFound() : TypedResults.Ok(nodesGroupedByCountry);
    })
    .WithName("GetNumberOfNodesGroupedByCountry")
    .WithSummary("Get number of nodes grouped by country for a specific date.")
    .WithOpenApi();

app.Run();