using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

/// <summary>
/// Peer info object
/// </summary>
public class Peer
{
    [JsonPropertyName("address")]
    public Address Address { get; set; }
        
    /// <summary>
    /// The base64Url (without padding) encoded SHA256 fingerprint of the SSL certificate of the node.
    /// This can be null only if the node uses an official CA signed certificate.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore]
    public string ChainwebNodeVersion { get; set; }

    [JsonIgnore]
    public bool? IsOnline { get; set; }
}