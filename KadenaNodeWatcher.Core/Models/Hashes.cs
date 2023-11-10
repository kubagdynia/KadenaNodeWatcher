using System.Text.Json.Serialization;

namespace KadenaNodeWatcher.Core.Models;

public class Hashes
{
    [JsonPropertyName("0")]
    public ChainHash ChainHash0 { get; set; }

    [JsonPropertyName("1")]
    public ChainHash ChainHash1 { get; set; }

    [JsonPropertyName("3")]
    public ChainHash ChainHash2 { get; set; }

    [JsonPropertyName("4")]
    public ChainHash ChainHash4 { get; set; }

    [JsonPropertyName("5")]
    public ChainHash ChainHash5 { get; set; }

    [JsonPropertyName("6")]
    public ChainHash ChainHash6 { get; set; }

    [JsonPropertyName("7")]
    public ChainHash ChainHash7 { get; set; }

    [JsonPropertyName("8")]
    public ChainHash ChainHash8 { get; set; }

    [JsonPropertyName("9")]
    public ChainHash ChainHash9 { get; set; }

    [JsonPropertyName("10")]
    public ChainHash ChainHash10 { get; set; }

    [JsonPropertyName("12")]
    public ChainHash ChainHash12 { get; set; }

    [JsonPropertyName("13")]
    public ChainHash ChainHash13 { get; set; }

    [JsonPropertyName("14")]
    public ChainHash ChainHash14 { get; set; }

    [JsonPropertyName("15")]
    public ChainHash ChainHash15 { get; set; }

    [JsonPropertyName("16")]
    public ChainHash ChainHash16 { get; set; }

    [JsonPropertyName("17")]
    public ChainHash ChainHash17 { get; set; }

    [JsonPropertyName("18")]
    public ChainHash ChainHash18 { get; set; }

    [JsonPropertyName("19")]
    public ChainHash ChainHash19 { get; set; }
}