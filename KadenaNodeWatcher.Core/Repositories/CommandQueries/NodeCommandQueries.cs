namespace KadenaNodeWatcher.Core.Repositories.CommandQueries;

internal class NodeCommandQueries : INodeCommandQueries
{
    public string AddNode
        => "INSERT INTO Nodes (IpAddress, Hostname, Port, IsOnline, NodeVersion) VALUES (@IpAddress, @Hostname, @Port, @IsOnline, @NodeVersion)";
    
    public string GetNumberOfNodes(bool? isOnline = null)
        =>  $"SELECT count(*) FROM Nodes WHERE Created = @date {(isOnline.HasValue ? "IsOnline = @isOnline" : "")}";

    public string GetNumberOfNodesGroupedByDates()
        => """
           SELECT
              n.Created AS Date, count(*) AS TotalCount,
              sum(CASE WHEN n.IsOnline = 1 THEN 1 ELSE 0 END) AS Online,
              sum(CASE WHEN n.IsOnline <> 1 THEN 1 ELSE 0 END) AS Offline
           FROM Nodes n WHERE n.Created BETWEEN @dateFrom AND @dateTo
           GROUP BY n.Created ORDER By Created ASC
           """;

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

    public string GetNodesWithoutIpGeolocation(int numberOfRecords)
        => $"""
           SELECT n.IpAddress, n.Hostname FROM Nodes n
           LEFT JOIN IpGeolocation ip ON ip.IpAddress = n.IpAddress
           WHERE n.IpAddress IS NOT NULL AND ip.Id IS NULL
           GROUP BY n.IpAddress, n.Hostname
           LIMIT {numberOfRecords}
           """;
}  