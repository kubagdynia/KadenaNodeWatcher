namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

public class NodeCommandQueries : INodeCommandQueries
{
    public string AddNode
        => @"INSERT INTO Nodes (IpAddress, Hostname, Port, IsOnline, NodeVersion) VALUES (@IpAddress, @Hostname, @Port, @IsOnline, @NodeVersion)";
    
    public string CountNodes(bool? isOnline = null)
        => isOnline.HasValue
            ? @"SELECT count(*) FROM Nodes WHERE Created = @date AND IsOnline = @isOnline"
            : @"SELECT count(*) FROM Nodes WHERE Created = @date";

    public string IpGeolocationExists
        => @"SELECT EXISTS (SELECT 1 FROM IpGeolocation WHERE IpAddress = @IpAddress)";

    public string AddIpGeolocation
        => @"INSERT INTO IpGeolocation (IpAddress, City, Country, CountryCode, CountryCodeIso3, CountryName, ContinentCode, RegionCode, Region, Org)
             VALUES (@IpAddress, @City, @Country, @CountryCode, @CountryCodeIso3, @CountryName, @ContinentCode, @RegionCode, @Region, @Org)";
}  