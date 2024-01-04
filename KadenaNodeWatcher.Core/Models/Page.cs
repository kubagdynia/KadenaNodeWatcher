using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

internal class Page
{
    /// <summary>
    /// A cursor that can be used to query the next page.
    /// It should be used literally as value for the next parameter in a follow-up request.
    /// </summary>
    [JsonPropertyName("next")]
    public string Next { get; set; }

    /// <summary>
    /// The number of items in the page.
    /// This number can be smaller but never be larger than the number of requested items.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
        
    /// <summary>
    /// Array of peers
    /// </summary>
    [JsonPropertyName("items")]
    public List<Peer> Items { get; set; }
}