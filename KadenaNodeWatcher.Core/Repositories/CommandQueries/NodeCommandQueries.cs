namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal class NodeCommandQueries : INodeCommandQueries
{
    public string AddNode
        => "INSERT INTO Nodes (IpAddress, Hostname, Port, IsOnline, NodeVersion) VALUES (@IpAddress, @Hostname, @Port, @IsOnline, @NodeVersion)";
    
    public string GetNumberOfNodes(bool? isOnline = null)
        =>  $"SELECT count(*) FROM Nodes WHERE Created = @date {(isOnline.HasValue ? "IsOnline = @isOnline" : "")}";

    public string GetNumberOfNodesGroupedByCountry(bool? isOnline = null)
        => $"""
            SELECT ip.CountryName, ip.CountryCode, COUNT(n.Id) Count  FROM Nodes n
            LEFT JOIN IpGeolocation ip ON ip.IpAddress = n.IpAddress WHERE n.Created = @date {(isOnline.HasValue ? "AND n.IsOnline = @isOnline" : "")}
            GROUP BY ip.Country ORDER BY COUNT(n.Id) DESC
            """;

    public string GetNodes(bool? isOnline = null)
        => $"""
            SELECT n.*, ip.CountryName, ip.CountryCode, ip.City, ip.ContinentCode, ip.Org FROM Nodes n
            LEFT JOIN IpGeolocation ip ON ip.IpAddress = n.IpAddress WHERE n.Created = @date {(isOnline.HasValue ? "AND n.IsOnline = @isOnline" : "")}
            """;

    public string IpGeolocationExists
        => "SELECT EXISTS (SELECT 1 FROM IpGeolocation WHERE IpAddress = @IpAddress)";

    public string GetIpGeolocation
        => """
           SELECT Id, IpAddress, City, Country,CountryCode, CountryCodeIso3, CountryName, ContinentCode, RegionCode, Region, Org
           FROM IpGeolocation WHERE IpAddress = @IpAddress
           """;

    public string AddIpGeolocation
        => """
           INSERT INTO IpGeolocation (IpAddress, City, Country, CountryCode, CountryCodeIso3, CountryName, ContinentCode, RegionCode, Region, Org)
           VALUES (@IpAddress, @City, @Country, @CountryCode, @CountryCodeIso3, @CountryName, @ContinentCode, @RegionCode, @Region, @Org)
           """;
    
}  