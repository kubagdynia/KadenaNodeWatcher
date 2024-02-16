using System.Net.Http.Json;
using System.Text.Json;
using KadenaNodeWatcher.Core.Configuration;
using KadenaNodeWatcher.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;

namespace KadenaNodeWatcher.Core.Chainweb;

internal class ChainwebNodeService : IChainwebNodeService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ChainwebNodeService> _logger;
    private readonly ChainwebSettings _chainwebSettings;
    private readonly IChainwebCommon _chainwebCommon;
    
    private readonly NodeVersion _nodeVersion;
    private readonly string _nodeApiVersion;

    public ChainwebNodeService(
        IHttpClientFactory clientFactory,
        IOptions<ChainwebSettings> chainwebSettings,
        ILogger<ChainwebNodeService> logger,
        IChainwebCommon chainwebCommon)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _chainwebSettings = chainwebSettings.Value;
        _chainwebCommon = chainwebCommon;
        
        var networkConfig = _chainwebSettings.GetSelectedNetworkConfig();
        
        _nodeVersion = networkConfig.NodeVersion;
        _nodeApiVersion = networkConfig.NodeApiVersion;
    }

    /// <summary>
    /// Query the current cut from a Chainweb node.
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<GetCutResponse> GetCutAsync(string baseAddress, CancellationToken ct = default)
    {
        var requestUri = $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut";
        
        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        try
        {
            using var response = await client.GetAsync(requestUri, ct);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var getCutResponse = new GetCutResponse
                    {
                        ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
                        Cut = await response.Content.ReadFromJsonAsync<Cut>(cancellationToken: ct)
                    };

                    return getCutResponse;

                }
                catch (NotSupportedException ex) // When content type is not valid
                {
                    _logger.LogError(ex, $"The content type is not supported. Node address: {baseAddress}");
                    throw;
                }
                catch (JsonException ex) // Invalid JSON
                {
                    _logger.LogError(ex, $"Invalid JSON. Node address: {baseAddress}");
                    throw;
                }
            }
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, $"Timeout has occurred. Node address: {baseAddress}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred. Node address: {baseAddress}");
            throw;
        }

        return await Task.FromResult<GetCutResponse>(null);
    }

    /// <summary>
    /// Returns an object containing the peers from the peer database of the remote node and base node headers
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Peers from the peer database of the remote node and base node headers</returns>
    public async Task<GetCutNetworkPeerInfoResponse> GetCutNetworkPeerInfoAsync(string baseAddress, CancellationToken ct = default)
    {
        // limit - Maximum number of records that may be returned.
        var requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit={_chainwebSettings.PageLimit}";
        
        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        try
        {
            using var response = await client.GetAsync(requestUri, ct);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var getCutNetworkPeerInfoResponse = new GetCutNetworkPeerInfoResponse
                    {
                        ResponseHeaders = _chainwebCommon.GetResponseHeaders(response),
                        Page = await response.Content.ReadFromJsonAsync<Page>(cancellationToken: ct)
                    };
                    
                    // check next page if needed
                    if (_chainwebSettings.CheckNextPage)
                    {
                        getCutNetworkPeerInfoResponse.Page.Items.AddRange(
                            await GetCutNetworkPeerInfoAsync(baseAddress, getCutNetworkPeerInfoResponse.Page.Next, ct));
                    }

                    return getCutNetworkPeerInfoResponse;

                }
                catch (NotSupportedException ex) // When content type is not valid
                {
                    _logger.LogError(ex, $"The content type is not supported. Node address: {baseAddress}");
                    throw;
                }
                catch (JsonException ex) // Invalid JSON
                {
                    _logger.LogError(ex, $"Invalid JSON. Node address: {baseAddress}");
                    throw;
                }
            }
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, $"Timeout has occurred. Node address: {baseAddress}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred. Node address: {baseAddress}");
            throw;
        }

        return await Task.FromResult<GetCutNetworkPeerInfoResponse>(null);
    }
    
    /// <summary>
    /// Returns peers from the peer database of the remote node
    /// </summary>
    /// <param name="baseAddress"></param>
    /// <param name="next">The cursor for the next page. This value can be found as value of the next property of the previous page.</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>Peers from the peer database of the remote node</returns>
    private async Task<List<Peer>> GetCutNetworkPeerInfoAsync(string baseAddress, string next, CancellationToken ct = default)
    {
        var items = new List<Peer>();

        if (string.IsNullOrEmpty(next))
        {
            return items;
        }

        var requestUri =
            $"{baseAddress}/chainweb/{_nodeApiVersion}/{_nodeVersion}/cut/peer?limit={_chainwebSettings.PageLimit}&next={next}";

        var client = _clientFactory.CreateClient("ClientWithoutSSLValidation");

        using var peers = await client.GetAsync(requestUri, ct);
            
        if (peers.IsSuccessStatusCode)
        {
            var page = await peers.Content.ReadFromJsonAsync<Page>(cancellationToken: ct);
            // Add an array of child peers to the result
            items.AddRange(page.Items);
            // If there is a next page, read this data.
            // Read the following pages until you have read them all.
            if (!string.IsNullOrEmpty(page.Next))
            {
                items.AddRange(await GetCutNetworkPeerInfoAsync(baseAddress, page.Next, ct));
            }
        }

        return items;
    }
}