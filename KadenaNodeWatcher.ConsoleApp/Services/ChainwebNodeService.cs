using System.Net.Http.Json;
using System.Text.Json;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;

namespace KadenaNodeWatcher.ConsoleApp.Services;

public class ChainwebNodeService : IChainwebNodeService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<App> _logger;
    private readonly AppSettings _appSettings;
    private readonly IChainwebCommon _chainwebCommon;
    
    private readonly NodeVersion _nodeVersion;
    private readonly string _nodeApiVersion;
    
    private readonly string _baseAddress;

    public ChainwebNodeService(
        IHttpClientFactory clientFactory,
        IOptions<AppSettings> appSettings,
        ILogger<App> logger,
        IChainwebCommon chainwebCommon)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _appSettings = appSettings.Value;
        _chainwebCommon = chainwebCommon;
        
        NetworkConfig networkConfig = _appSettings.GetSelectedNetworkConfig();
        
        _nodeVersion = networkConfig.NodeVersion;
        _nodeApiVersion = networkConfig.NodeApiVersion;
        
        string node = _appSettings.GetSelectedRootNode();
        _logger.LogInformation($"Selected root node: {node}.");
        
        _baseAddress = node;
    }

    /// <summary>
    /// Query the current cut from a Chainweb node.
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<GetCutResponse> GetCutAsync(string baseAddress)
    {
        string requestUri = $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut";
        
        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        try
        {
            using HttpResponseMessage response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var getCutResponse = new GetCutResponse
                    {
                        ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
                        Cut = await response.Content.ReadFromJsonAsync<Cut>()
                    };

                    return getCutResponse;

                }
                catch (NotSupportedException ex) // When content type is not valid
                {
                    _logger.LogError(ex, "The content type is not supported.");
                    throw;
                }
                catch (JsonException ex) // Invalid JSON
                {
                    _logger.LogError(ex, "Invalid JSON.");
                    throw;
                }
            }
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout has occurred.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
            throw;
        }

        return await Task.FromResult<GetCutResponse>(null);
    }

    public async Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync()
    {
        return await GetCutNetworkPeerInfoAsync(_baseAddress);
    }

    public async Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress)
    {
        string requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit={_appSettings.PageLimit}";
        
        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        try
        {
            using HttpResponseMessage response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var getCutNetworkPeerInfoResponse = new GetCutNetworkPeerInfoResponse
                    {
                        ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
                        Page = await response.Content.ReadFromJsonAsync<Page>()
                    };
                    
                    // check next page if needed
                    if (_appSettings.CheckNextPage)
                    {
                        getCutNetworkPeerInfoResponse.Page.Items.AddRange(
                            await GetCutNetworkPeerInfoAsync(baseAddress, getCutNetworkPeerInfoResponse.Page.Next));
                    }

                    return getCutNetworkPeerInfoResponse;

                }
                catch (NotSupportedException ex) // When content type is not valid
                {
                    _logger.LogError(ex, "The content type is not supported.");
                    throw;
                }
                catch (JsonException ex) // Invalid JSON
                {
                    _logger.LogError(ex, "Invalid JSON.");
                    throw;
                }
            }
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout has occurred.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
            throw;
        }

        return await Task.FromResult<GetCutNetworkPeerInfoResponse>(null);
    }
    
    private async Task<List<Peer>> GetCutNetworkPeerInfoAsync(string baseAddress, string next)
    {
        List<Peer> items = new List<Peer>();

        if (string.IsNullOrEmpty(next))
        {
            return items;
        }

        string requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit={_appSettings.PageLimit}&next={next}";

        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        using HttpResponseMessage response = await client.GetAsync(requestUri);
            
        if (response.IsSuccessStatusCode)
        {
            Page page = await response.Content.ReadFromJsonAsync<Page>();
            items.AddRange(page.Items);
            if (!string.IsNullOrEmpty(page.Next))
            {
                items.AddRange(await GetCutNetworkPeerInfoAsync(baseAddress, page.Next));
            }
        }

        return items;
    }
}