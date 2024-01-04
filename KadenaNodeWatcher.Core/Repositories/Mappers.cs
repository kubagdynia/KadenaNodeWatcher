using IpGeolocation.Models;
using KadenaNodeWatcher.Core.Repositories.DbModels;

namespace KadenaNodeWatcher.Core.Repositories;

internal static class Mappers
{
    internal static IpGeolocationDb ToDbModel(this IpGeolocationModel ipGeolocationModel)
        => new()
        {
            IpAddress = ipGeolocationModel.Ip,
            City = ipGeolocationModel.City,
            Country = ipGeolocationModel.Country,
            CountryCode = ipGeolocationModel.CountryCode,
            CountryCodeIso3 = ipGeolocationModel.CountryCodeIso3,
            CountryName = ipGeolocationModel.CountryName,
            RegionCode = ipGeolocationModel.RegionCode,
            Region = ipGeolocationModel.Region,
            ContinentCode = ipGeolocationModel.ContinentCode,
            Org = ipGeolocationModel.Org
        };

    internal static IpGeolocationModel ToApiModel(this IpGeolocationDb ipGeolocationDb)
        => new()
        {
            Ip = ipGeolocationDb.IpAddress,
            City = ipGeolocationDb.City,
            Country = ipGeolocationDb.Country,
            CountryCode = ipGeolocationDb.CountryCode,
            CountryCodeIso3 = ipGeolocationDb.CountryCodeIso3,
            CountryName = ipGeolocationDb.CountryName,
            Region = ipGeolocationDb.Region,
            RegionCode = ipGeolocationDb.RegionCode,
            ContinentCode = ipGeolocationDb.ContinentCode,
            Org = ipGeolocationDb.Org
        };
}