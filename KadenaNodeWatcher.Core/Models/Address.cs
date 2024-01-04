using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

internal class Address
{
    /// <summary>
    /// A domain name or IP address.
    /// This must be a domain name only if the respective node is using a valid CA signed SSL certificate.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string Hostname { get; set; }

    /// <summary>
    /// Port number (min: 1, max: 65535)
    /// </summary>
    [JsonPropertyName("port")]
    public int Port { get; set; }

    /// <summary>
    /// IP address
    /// </summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; }
}