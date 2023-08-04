namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.Commons
{
    public class ResponseUniversitiesCommons
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Acronym { get; set; }
        public string LogoImage { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string FaxNumber { get; set; }
        public string FaxNumber1 { get; set; }
        public string FaxNumber2 { get; set; }
        public string LegalNoticeUrl { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string LegalNoticeContent { get; set; }
        public string PrivacyPolicyContent { get; set; }
        public string CookiePolicyUrl { get; set; }
        public string CookiePolicyContent { get; set; }
        public string ContactEmail { get; set; }
        public long CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string DefaultCulture { get; set; }
    }
}
