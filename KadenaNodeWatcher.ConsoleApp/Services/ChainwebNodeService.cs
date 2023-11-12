using System.Net.Http.Json;
using KadenaNodeWatcher.Core.Chainweb;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Models;

namespace KadenaNodeWatcher.ConsoleApp.Services;

public class ChainwebNodeService : IChainwebNodeService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IChainwebCommon _chainwebCommon;
    
    private readonly NodeVersion _nodeVersion;
    private readonly string _nodeApiVersion;

    public ChainwebNodeService(
        IHttpClientFactory clientFactory,
        IChainwebCommon chainwebCommon)
    {
        _clientFactory = clientFactory;
        _chainwebCommon = chainwebCommon;
        
        _nodeVersion = NodeVersion.mainnet01;
        _nodeApiVersion = "0.0";
    }

    public async Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress)
    {
        string requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit=250";
        
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