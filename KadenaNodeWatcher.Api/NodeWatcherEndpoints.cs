using KadenaNodeWatcher.Core.Models.Dto;
using KadenaNodeWatcher.Core.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace KadenaNodeWatcher.Api;

public static class NodeWatcherEndpoints
{
    public static void MapNodeWatcherEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder route = app.MapGroup("kadenanodes/api/v1");
     
        // GET /kadenanodes/api/v1/nodes
        GetFullNodesDataForASpecificDate(route);

        // GET /kadenanodes/api/v1/nodes/count
        GetNumberOfNodesGroupedByDates(route);

        // GET /kadenanodes/api/v1/nodes/countries/count
        GetNumberOfNodesGroupedByCountryForASpecificDate(route);

        // GET /kadenanodes/api/v1/nodes/versions/count
        GetNumberOfNodesGroupedByVersionForASpecificDate(route);
    }

    private static void GetFullNodesDataForASpecificDate(RouteGroupBuilder route)
    {
        route.MapGet("/nodes", async Task<Results<Ok<IEnumerable<FullNodeDataDto>>, NotFound, BadRequest>>
                (DateTime? date, bool? isOnline, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
            {
                date ??= DateTime.Now;

                IEnumerable<FullNodeDataDto> nodes = (await kadenaNodeWatcherService.GetNodes(date.Value, isOnline)).ToList();
                return !nodes.Any() ? TypedResults.NotFound() : TypedResults.Ok(nodes);
            })
            .WithName("GetNodes")
            .WithSummary("Get full nodes data for a specific date.")
            .CacheOutput()
            .WithOpenApi();
    }
    
    private static void GetNumberOfNodesGroupedByDates(RouteGroupBuilder route)
    {
        route.MapGet("/nodes/count", async Task<Results<Ok<IEnumerable<NumberOfNodesGroupedByDatesDto>>, NotFound, BadRequest>>
                (DateTime? dateFrom, DateTime? dateTo, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
            {
                dateFrom ??= DateTime.Now;
                dateTo ??= dateFrom;

                IEnumerable<NumberOfNodesGroupedByDatesDto> numberOfNodesGroupedByDates =
                    (await kadenaNodeWatcherService.GetNumberOfNodesGroupedByDates(dateFrom.Value, dateTo.Value)).ToList();
        
                return !numberOfNodesGroupedByDates.Any() ? TypedResults.NotFound() : TypedResults.Ok(numberOfNodesGroupedByDates);
            })
            .WithName("GetNumberOfNodesGroupedByDates")
            .WithSummary("Get number of nodes grouped by dates.")
            .CacheOutput()
            .WithOpenApi();
    }
    
    private static void GetNumberOfNodesGroupedByCountryForASpecificDate(RouteGroupBuilder route)
    {
        route.MapGet("/nodes/countries/count", async Task<Results<Ok<IEnumerable<NumberOfNodesGroupedByCountryDto>>, NotFound, BadRequest>>
                (DateTime? date, bool? isOnline, string? nodeVersion, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
            {
                date ??= DateTime.Now;

                IEnumerable<NumberOfNodesGroupedByCountryDto> nodesGroupedByCountry =
                    (await kadenaNodeWatcherService.GetNumberOfNodesGroupedByCountry(date.Value, isOnline, nodeVersion)).ToList();
        
                return !nodesGroupedByCountry.Any() ? TypedResults.NotFound() : TypedResults.Ok(nodesGroupedByCountry);
            })
            .WithName("GetNumberOfNodesGroupedByCountry")
            .WithSummary("Get number of nodes grouped by country for a specific date.")
            .CacheOutput()
            .WithOpenApi();
    }
    
    private static void GetNumberOfNodesGroupedByVersionForASpecificDate(RouteGroupBuilder route)
    {
        route.MapGet("/nodes/versions/count", async Task<Results<Ok<IEnumerable<NumberOfNodesGroupedByVersionDto>>, NotFound, BadRequest>>
                (DateTime? date, bool? isOnline, IKadenaNodeWatcherService kadenaNodeWatcherService) =>
            {
                date ??= DateTime.Now;

                IEnumerable<NumberOfNodesGroupedByVersionDto> nodesGroupedByVersion =
                    (await kadenaNodeWatcherService.GetNumberOfNodesGroupedByVersion(date.Value, isOnline)).ToList();
        
                return !nodesGroupedByVersion.Any() ? TypedResults.NotFound() : TypedResults.Ok(nodesGroupedByVersion);
            })
            .WithName("GetNumberOfNodesGroupedByVersion")
            .WithSummary("Get number of nodes grouped by version for a specific date.")
            .CacheOutput()
            .WithOpenApi();
    }
}