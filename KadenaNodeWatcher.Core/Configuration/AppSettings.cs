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
}