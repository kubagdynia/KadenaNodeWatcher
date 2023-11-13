namespace KadenaNodeWatcher.Core.Configuration;

public class AppSettings
{
    public int PageLimit { get; set; }

    public bool CheckNextPage { get; set; }

    public Network Network { get; set; }

    public NetworkConfig Mainnet { get; set; }

    public NetworkConfig Testnet { get; set; }

    public NetworkConfig GetSelectedNetworkConfig()
        => Network == Network.Mainnet ? Mainnet : Testnet;
    
    public string GetSelectedRootNode()
    {
        NetworkConfig networkConfig = GetSelectedNetworkConfig();
            
        if (networkConfig == null)
        {
            return null;
        }

        if (!networkConfig.RootNodes.Any())
        {
            return null;
        }

        int selectedRootNode = networkConfig.SelectedRootNode ?? 1;

        if (selectedRootNode > networkConfig.RootNodes.Count)
        {
            selectedRootNode = networkConfig.RootNodes.Count;
        }
        else if (selectedRootNode < 1)
        {
            selectedRootNode = 1;
        }

        string node = networkConfig.RootNodes[selectedRootNode - 1];

        return node;
    }
}