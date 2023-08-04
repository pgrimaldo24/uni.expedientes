using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.GestorDocumental;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorDocumental;
using Unir.Expedientes.Infrastructure.ServicesClients.Parameters.GestorDocumental;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class GestorDocumentalServiceClient : IGestorDocumentalServiceClient
    {
        private const string DocumentosApi = "api/v1/documentos";
        private const string ClasificacionesApi = "api/v1/clasificaciones-documentales";

        private readonly HttpClient _client;
        private readonly ILogger<GestorDocumentalServiceClient> _logger;
        private readonly ISecurityIntegrationServices _securityCrosscuttingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<GestorDocumentalServiceClient> _localizer;
        private readonly IMapper _mapper;

        public GestorDocumentalServiceClient(
            IHttpClientFactory clientFactory,
            ILogger<GestorDocumentalServiceClient> logger,
            ISecurityIntegrationServices securityCrosscuttingService,
            IOptions<ServicesClientsSettings> clientSettings,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<GestorDocumentalServiceClient> localizer)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.GestorDocumentalServices.Host);
            _logger = logger;
            _securityCrosscuttingService = securityCrosscuttingService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _mapper = ConfigureMapping();
        }

        protected Mapper ConfigureMapping()
        {
            return new Mapper(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseDocumento, DocumentoModel>();
                conf.CreateMap<ResponseClasificacion, ClasificacionModel>();
            }));
        }

        protected internal virtual async Task SetAuthorizationClient()
        {
            var resultToken = await _securityCrosscuttingService.GetTokenGestorDocumental();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultToken);
        }

        public async Task<DocumentoModel> SaveDocumento(DocumentoParameters documento)
        {
            await SetAuthorizationClient();
            var urlBase = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var parameters = new
            {
                documento.Referencia,
                NombreFichero = documento.Nombre,
                Url = $"{urlBase}/{documento.Url}",
                ClasificacionDocumental = new
                {
                    Id = documento.IdRefClasificacion
                }
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync(DocumentosApi, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(parameters));
                _logger.LogError(mensajeError);
                throw new BadRequestException(
                    _localizer["Gestor Documental: No se ha podido registrar el documento"]);
            }

            var documentoResponse = await response.Content.ReadAsAsync<ResponseDocumento>();
            return _mapper.Map<ResponseDocumento, DocumentoModel>(documentoResponse);
        }

        public async Task<List<ClasificacionModel>> GetClasificaciones(ClasificacionListParameters clasificacionParameters)
        {
            await SetAuthorizationClient();
            var parameters = new ClasificacionAdvancedSearchParameters
            {
                FilterCodigo = clasificacionParameters.FilterCodigo,
                PageIndex = 1,
                ItemsPerPage = 10
            };
            var endPoint = $"{ClasificacionesApi}/advanced-search";
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(parameters));
                _logger.LogError(mensajeError);
                return new List<ClasificacionModel>();
            }

            var clasificacionesResponse = await response.Content.ReadAsAsync<List<ResponseClasificacion>>();
            return _mapper.Map<List<ResponseClasificacion>, List<ClasificacionModel>>(clasificacionesResponse);
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio Gestor Documental : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
            ticks,
            response.RequestMessage.RequestUri,
            response.RequestMessage.Method,
            response.RequestMessage.Headers,
            body,
            await response.Content.ReadAsStringAsync(),
            response.StatusCode);
            return mensajeError;
        }
    }
}
