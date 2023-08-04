namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.Commons
{
    public class ResponseLevelDivisionCommon
    {
        public string Code { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string DivisionCode { get; set; }
        public int DivisionLevel { get; set; }
        public string DivisionName { get; set; }
        public bool IsPreferredDivision { get; set; }
        public string IsoCode { get; set; }
        public string LocalCode { get; set; }
        public string Name { get; set; }
        public string ParentEntityCode { get; set; }
    }
}
