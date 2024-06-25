namespace KadenaNodeWatcher.Core.Configuration;

public class ChainwebSettings
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
        var networkConfig = GetSelectedNetworkConfig();
        
        var selectedRootNodeIndex = GetSelectedRootNodeIndex(networkConfig);

        if (selectedRootNodeIndex < 0)
        {
            return null;
        }
        
        return networkConfig.RootNodes[selectedRootNodeIndex];
    }

    public List<string> GetNotSelectedRootNodes()
    {
        var networkConfig = GetSelectedNetworkConfig();
        
        var selectedRootNodeIndex = GetSelectedRootNodeIndex(networkConfig);

        var endIndex = networkConfig.RootNodes.Count - 1;

        List<string> resultList = [];
        if (endIndex > selectedRootNodeIndex)
        {
            for (var i = selectedRootNodeIndex + 1; i <= endIndex; i++)
            {
                resultList.Add(networkConfig.RootNodes[i]);
            }
        }

        if (selectedRootNodeIndex > 0)
        {
            for (var i = 0; i < selectedRootNodeIndex; i++)
            {
                resultList.Add(networkConfig.RootNodes[i]);
            }
        }

        return resultList;
    }
    
    private int GetSelectedRootNodeIndex(NetworkConfig networkConfig)
    {
        if (networkConfig == null)
        {
            return -1;
        }

        if (networkConfig.RootNodes.Count == 0)
        {
            return -1;
        }

        var selectedRootNode = networkConfig.SelectedRootNode ?? 1;

        if (selectedRootNode > networkConfig.RootNodes.Count)
        {
            selectedRootNode = networkConfig.RootNodes.Count;
        }
        else if (selectedRootNode < 1)
        {
            selectedRootNode = 1;
        }

        return selectedRootNode - 1;
    }
}