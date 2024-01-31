namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal class NodeCommandQueries : INodeCommandQueries
{
    public string AddNode
        => "INSERT INTO Nodes (IpAddress, Hostname, Port, IsOnline, NodeVersion) VALUES (@IpAddress, @Hostname, @Port, @IsOnline, @NodeVersion)";
    
    public string GetNumberOfNodes(bool? isOnline = null)
        => isOnline.HasValue
            ? "SELECT count(*) FROM Nodes WHERE Created = @date AND IsOnline = @isOnline"
            : "SELECT count(*) FROM Nodes WHERE Created = @date";

    public string GetNodes()
        => @"SELECT n.*, ip.CountryName, ip.CountryCode, ip.City, ip.ContinentCode, ip.Org FROM Nodes n
             LEFT JOIN IpGeolocation ip ON ip.IpAddress = n.IpAddress WHERE n.Created = @date";

    public string IpGeolocationExists
        => "SELECT EXISTS (SELECT 1 FROM IpGeolocation WHERE IpAddress = @IpAddress)";

    public string GetIpGeolocation
        => @"SELECT Id, IpAddress, City, Country,CountryCode, CountryCodeIso3, CountryName, ContinentCode, RegionCode, Region, Org
             FROM IpGeolocation WHERE IpAddress = @IpAddress";

    public string AddIpGeolocation
        => @"INSERT INTO IpGeolocation (IpAddress, City, Country, CountryCode, CountryCodeIso3, CountryName, ContinentCode, RegionCode, Region, Org)
             VALUES (@IpAddress, @City, @Country, @CountryCode, @CountryCodeIso3, @CountryName, @ContinentCode, @RegionCode, @Region, @Org)";
}  