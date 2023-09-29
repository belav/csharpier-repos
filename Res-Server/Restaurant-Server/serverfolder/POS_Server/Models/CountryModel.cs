using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POS_Server.Models
{
    public class CountryModel
    {
        public long countryId { get; set; }
        public string code { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public byte isDefault { get; set; }
        public long currencyId { get; set; }
        public List<CityModel> citiesList { get; set; }
        public string timeZoneName { get; set; }
        public string timeZoneOffset { get; set; }
    }

    public class CityModel
    {
        public long cityId { get; set; }
        public string cityCode { get; set; }
        public Nullable<long> countryId { get; set; }
    }
}
