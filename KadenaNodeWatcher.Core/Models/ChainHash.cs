using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

internal class ChainHash
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }
}