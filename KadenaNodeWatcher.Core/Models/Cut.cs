using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

public class Cut
{
    /// <summary>
    /// Peer info object
    /// </summary>
    [JsonPropertyName("origin")]
    public Peer Origin { get; set; }

    /// <summary>
    /// The cut height is the sum of the height of all blocks of the cut.
    /// Usage of this value should be avoided, because its semantics may change in the future
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// The sum of the weights of all blocks in the cut
    /// </summary>
    [JsonPropertyName("weight")]
    public string Weight { get; set; }

    /// <summary>
    /// An object that maps chain Ids to their respective block hash and block height
    /// </summary>
    [JsonPropertyName("hashes")]
    public Hashes Hashes { get; set; }
}