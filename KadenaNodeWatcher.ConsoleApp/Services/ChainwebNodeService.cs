using System.Net.Http.Json;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Models;
using Microsoft.Extensions.Options;

namespace KadenaNodeWatcher.ConsoleApp.Services;

public class ChainwebNodeService : IChainwebNodeService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly AppSettings _appSettings;
    private readonly IChainwebCommon _chainwebCommon;
    
    private readonly NodeVersion _nodeVersion;
    private readonly string _nodeApiVersion;

    public ChainwebNodeService(
        IHttpClientFactory clientFactory,
        IOptions<AppSettings> appSettings,
        IChainwebCommon chainwebCommon)
    {
        _clientFactory = clientFactory;
        _appSettings = appSettings.Value;
        _chainwebCommon = chainwebCommon;
        
        NetworkConfig networkConfig = _appSettings.GetSelectedNetworkConfig();
        
        _nodeVersion = networkConfig.NodeVersion;
        _nodeApiVersion = networkConfig.NodeApiVersion;
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
        
        using HttpResponseMessage response = await client.GetAsync(requestUri);
        
        var getCutResponse = new GetCutResponse
        {
            ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
            Cut = await response.Content.ReadFromJsonAsync<Cut>()
        };

        return getCutResponse;
    }

    public async Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress)
    {
        string requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit={_appSettings.PageLimit}";
        
        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");
        
        using HttpResponseMessage response = await client.GetAsync(requestUri);
        
        var getCutNetworkPeerInfoResponse = new GetCutNetworkPeerInfoResponse
        {
            ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
            Page = await response.Content.ReadFromJsonAsync<Page>()
        };

        return getCutNetworkPeerInfoResponse;
    }
}