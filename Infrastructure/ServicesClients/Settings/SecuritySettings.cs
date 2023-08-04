namespace Unir.Expedientes.Infrastructure.ServicesClients.Settings
{
    public class ApiKeyConfigurationModel
    {
        public string Application { get; set; }
        public string ApiKey { get; set; }
    }
    public class OidcServerConfigurationModel
    {
        public string Uri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
    }
    public class SecurityConfigurationModel
    {
        public ApiKeyConfigurationModel[] AuthorizedApiKeys { get; set; }
        public OidcServerConfigurationModel OidcServer { get; set; }
    }
}
