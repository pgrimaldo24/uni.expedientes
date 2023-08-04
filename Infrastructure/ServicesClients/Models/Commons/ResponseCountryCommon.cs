namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.Commons
{
    public class ResponseCountryCommon
    { 
        public string IsoCode { get; set; }
        public string Name { get; set; }
        public string Iso3166Alpha3 { get; set; }
        public string Iso3166Numeric { get; set; }
        public string PhoneCode { get; set; }
        public string[] PreferredDivisions { get; set; }
    }
}
