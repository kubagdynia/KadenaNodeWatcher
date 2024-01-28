using Microsoft.Extensions.Configuration;

namespace KadenaNodeWatcher.Core.Configuration;

public class DefaultChainwebConfig
{
    private DefaultChainwebConfig()
    {
        
    }

    public static IConfigurationSection GetDefaultChainwebConfig(string chainwebConfigSectionName)
    {
        // Create default configuration
        var inMemorySettings = new Dictionary<string, string> {
            {$"{chainwebConfigSectionName}:PageLimit", "250"},
            {$"{chainwebConfigSectionName}:CheckNextPage", "true"},
            {$"{chainwebConfigSectionName}:Network", "mainnet"},
            {$"{chainwebConfigSectionName}:Mainnet:SelectedRootNode", "1"},
            {$"{chainwebConfigSectionName}:Mainnet:ChildNodes:RandomPick", "true"},
            {$"{chainwebConfigSectionName}:Mainnet:ChildNodes:Count", "7"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:0", "https://us-e1.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:1", "https://us-e2.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:2", "https://us-e3.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:3", "https://us-w1.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:4", "https://us-w2.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:5", "https://us-w3.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:6", "https://jp1.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:7", "https://jp2.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:8", "https://jp3.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:9", "https://fr1.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:10", "https://fr2.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:RootNodes:11", "https://fr3.chainweb.com"},
            {$"{chainwebConfigSectionName}:Mainnet:NodeVersion", "mainnet01"},
            {$"{chainwebConfigSectionName}:Mainnet:NodeApiVersion", "0.0"},
            {$"{chainwebConfigSectionName}:Testnet:SelectedRootNode", "1"},
            {$"{chainwebConfigSectionName}:Testnet:RootNodes:0", "https://us1.testnet.chainweb.com"},
            {$"{chainwebConfigSectionName}:Testnet:RootNodes:1", "https://us2.testnet.chainweb.com"},
            {$"{chainwebConfigSectionName}:Testnet:RootNodes:2", "https://eu1.testnet.chainweb.com"},
            {$"{chainwebConfigSectionName}:Testnet:RootNodes:3", "https://eu2.testnet.chainweb.com"},
            {$"{chainwebConfigSectionName}:Testnet:RootNodes:5", "https://ap2.testnet.chainweb.com"},
            {$"{chainwebConfigSectionName}:Testnet:NodeVersion", "testnet04"},
            {$"{chainwebConfigSectionName}:Testnet:NodeApiVersion", "0.0"},
        };

        IConfiguration defaultConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        IConfigurationSection configurationSection = defaultConfiguration.GetSection(chainwebConfigSectionName);
        return configurationSection;
    }
}