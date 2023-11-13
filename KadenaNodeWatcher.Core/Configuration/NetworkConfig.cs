namespace KadenaNodeWatcher.Core.Configuration;

public class NetworkConfig
{
    public int? SelectedRootNode { get; set; }

    public ChildNodes ChildNodes { get; set; } = new();

    public List<string> RootNodes { get; set; }

    /// <summary>
    /// Chainweb version to use (e.g. mainnet01 or testnet04)
    /// </summary>
    public NodeVersion NodeVersion { get; set; }

    public string NodeApiVersion { get; set; }

    public List<ExcludedNode> ExcludedNodes { get; set; }
}