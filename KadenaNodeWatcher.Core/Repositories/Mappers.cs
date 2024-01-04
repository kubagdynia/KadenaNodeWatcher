using IpGeolocation.Models;
using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

public static class Mappers
{
    public static IpGeolocationDb ToDbModel(this IpGeolocationModel ipGeolocationModel)
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