using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Security;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class SecurityServiceClient : ISecurityIntegrationServices
    {
        private static AuthTokenModel _cachedAccessSecurityServicesToken;

        private const int DefaultTokenTimeout = 1199;

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<SecurityServiceClient> _logger;
        private readonly ServicesClientsSettings _clientsSettings;
        private readonly SecurityConfigurationModel _securitySettings;

        public SecurityServiceClient(ILogger<SecurityServiceClient> logger,
            IOptions<ServicesClientsSettings> clientSettings,
            IOptions<SecurityConfigurationModel> securitySettings, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clientsSettings = clientSettings.Value;
            _securitySettings = securitySettings.Value;
        }

        public async Task<string> GetTokenServiciosDesUnirExpedientes()
        {
            return await GetTokenOAuthServices("expedientes", "expedientes_2018");
        }

        private async Task<string> GetTokenOAuthServices(string userName, string password)
        {
            if (_cachedAccessSecurityServicesToken != null && _cachedAccessSecurityServicesToken.ExpirationDateTime > DateTime.UtcNow)
            {
                return _cachedAccessSecurityServicesToken.AccessToken;
            }

            var serverUri = new Uri(_securitySettings.OidcServer.Uri);
            var clientId = _securitySettings.OidcServer.ClientId;
            var clientSecret = _securitySettings.OidcServer.ClientSecret;

            var httpClient = _clientFactory.CreateClient();
            var response = httpClient.PostAsync(new Uri(serverUri, "token"),
                new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret)
                    })).Result;

            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var message =
                    $"[{ticks}] Error al obtener token de autorización. Respuesta desde crosscutting: '{response.Content.ReadAsStringAsync().Result}'";
                _logger.Log(LogLevel.Error, message);
                return null;
            }

            var content = await response.Content.ReadFromJsonAsync<AuthTokenModel>();
            if (content == null)
            {
                return null;
            }

            if (!content.ExpiresIn.HasValue)
            {
                content.ExpiresIn = DefaultTokenTimeout;
                _logger.Log(LogLevel.Warning,
                    "La respuesta con el token de acceso a los servicios web no lleva información de Timeup");
            }

            content.ExpirationDateTime = DateTime.UtcNow.AddSeconds(content.ExpiresIn.Value);
            _cachedAccessSecurityServicesToken = content;
            return content.AccessToken;
        }

        public async Task<string> GetTokenErpAcademico()
        {
            return await GetServicesAuthToken(_clientsSettings.ErpAcademicoServices.Host, "integracion.erpacademico@unir.net",
                "ifintegracion");
        }

        public async Task<string> GetTokenGestorDocumental()
        {
            return await GetServicesAuthToken(_clientsSettings.GestorDocumentalServices.Host, 
                "integracion.expedientes@unir.net", "ifintegracion");
        }

        private async Task<string> GetServicesAuthToken(string hostApp, string userName, string password)
        {
            var uriHost = new Uri(hostApp);
            var httpClient = _clientFactory.CreateClient();
            var response = httpClient.PostAsync(new Uri(uriHost, "token"),
                new FormUrlEncodedContent(
                    new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", password)
                    })).Result;

            if (!response.IsSuccessStatusCode)
            {
                var message =
                    $"Error al obtener token de autorización. Respuesta desde crosscutting: '{response.Content.ReadAsStringAsync().Result}'";
                _logger.Log(LogLevel.Warning, message);
                return null;
            }

            var content = await response.Content.ReadFromJsonAsync<AuthTokenModel>();
            if (content == null)
            {
                return null;
            }
            
            if (!content.ExpiresIn.HasValue)
            {
                content.ExpiresIn = DefaultTokenTimeout;
                _logger.Log(LogLevel.Warning,
                    "La respuesta con el token de acceso a los servicios web no lleva información de Timeup");
            }

            content.ExpirationDateTime = DateTime.UtcNow.AddSeconds(content.ExpiresIn.Value);

            return content.AccessToken;
        }
    }
}
