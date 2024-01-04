using IpGeolocation.Models;

namespace KadenaNodeWatcher.Core.Models.NodeData;

public class IpGeo
{
    public string IpAddress { get; set; }
    
    public string City { get; set; }
    
    public string Country { get; set; }
    
    public string CountryCode { get; set; }
    
    public string CountryCodeIso3 { get; set; }
    
    public string CountryName { get; set; }
    
    public string ContinentCode { get; set; }
    
    public string RegionCode { get; set; }
    
    public string Region { get; set; }
    
    public string Org { get; set; }

    public static IpGeo CreateIpGeo(IpGeolocationModel ipGeolocationModel)
        => new()
        {
            IpAddress = ipGeolocationModel.Ip,
            City = ipGeolocationModel.City,
            Country = ipGeolocationModel.Country,
            CountryCode = ipGeolocationModel.CountryCode,
            CountryCodeIso3 = ipGeolocationModel.CountryCodeIso3,
            CountryName = ipGeolocationModel.CountryName,
            ContinentCode = ipGeolocationModel.ContinentCode,
            RegionCode = ipGeolocationModel.RegionCode,
            Region = ipGeolocationModel.Region,
            Org = ipGeolocationModel.Org
        };
}