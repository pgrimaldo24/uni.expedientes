using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorMapeos;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class GestorMapeosServiceClient : IGestorMapeosServiceClient
    {
        private const string MapeoEstudioApi = "api/v1/mapeo-estudio";
        private const string MapeoAsignaturaApi = "api/v1/mapeo-asignatura";

        private readonly HttpClient _client;
        private readonly ILogger<GestorMapeosServiceClient> _logger;
        private readonly IMapper _mapper;
        private readonly ISecurityIntegrationServices _securityCrosscuttingService;

        public GestorMapeosServiceClient(
            IHttpClientFactory clientFactory, 
            ILogger<GestorMapeosServiceClient> logger,
            ISecurityIntegrationServices securityCrosscuttingService, 
            IOptions<ServicesClientsSettings> clientSettings)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.GestorMapeosServices.Host);
            _logger = logger;
            _securityCrosscuttingService = securityCrosscuttingService;
            _mapper = ConfigureMapping();
        }

        protected internal virtual async Task SetAuthorizationClient()
        {
            var resultToken = await _securityCrosscuttingService.GetTokenErpAcademico();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultToken);
        }

        protected Mapper ConfigureMapping()
        {
            return new Mapper(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseEstudioGestorMapeo, EstudioGestorMapeoModel>();
                conf.CreateMap<ResponseAsignaturaGestorMapeo, AsignaturaGestorMapeoModel>();
                conf.CreateMap<ResponseAsignaturaGestorMapeoModel, AsignaturaIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponseAsignaturaUnirGestorMapeoModel, AsignaturaUnirIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponseAsignaturaPlanGestorMapeoModel, AsignaturaPlanIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponseEstudioUnirGestorMapeoModel, EstudioUnirIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponsePlanGestorMapeoModel, PlanIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponseEstudioGestorMapeoModel, EstudioIntegrationGestorMapeoModel>();
                conf.CreateMap<ResponsePlantillaEstudioIntegracionGestorMapeoModel, PlantillaEstudioIntegracionGestorMapeoModel>();
            }));
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio Gestor de Mapeos : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
            ticks,
            response.RequestMessage.RequestUri,
            response.RequestMessage.Method,
            response.RequestMessage.Headers,
            body,
            await response.Content.ReadAsStringAsync(),
            response.StatusCode);
            return mensajeError;
        }

        public virtual async Task<List<EstudioGestorMapeoModel>> GetEstudios(
            string idRefPlan, string idRefVersionPlan, int? idEstudioGestor = null)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MapeoEstudioApi}/search?idPlanErp={idRefPlan}" +
                $"&idRefVersionPlanErp={idRefVersionPlan}&idEstudioGestor={idEstudioGestor}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"{idRefPlan} - {idRefVersionPlan}");
                _logger.LogError(mensajeError);
                return null;
            }

            var estudios = await response.Content.ReadAsAsync<List<ResponseEstudioGestorMapeo>>();
            return _mapper.Map<List<ResponseEstudioGestorMapeo>, List<EstudioGestorMapeoModel>>(estudios);
        }

        public virtual async Task<List<AsignaturaGestorMapeoModel>> GetAsignaturas(
            int? idEstudioGestor = null, int? idAsignaturaEstudioGestor = null)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MapeoAsignaturaApi}/search?idEstudioGestor={idEstudioGestor}" +
                $"&idAsignaturaEstudioGestor={idAsignaturaEstudioGestor}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idEstudioGestor.ToString());
                _logger.LogError(mensajeError);
                return null;
            }

            var asignaturas = await response.Content.ReadAsAsync<List<ResponseAsignaturaGestorMapeo>>();
            return _mapper.Map<List<ResponseAsignaturaGestorMapeo>, List<AsignaturaGestorMapeoModel>>(asignaturas);
        }

        public virtual async Task<AsignaturaIntegrationGestorMapeoModel> GetAsignatura(int idAsignaturaEstudioGestor)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MapeoAsignaturaApi}/{idAsignaturaEstudioGestor}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idAsignaturaEstudioGestor.ToString());
                _logger.LogError(mensajeError);
                return null;
            }

            var asignatura = await response.Content.ReadAsAsync<ResponseAsignaturaGestorMapeoModel>();
            return _mapper.Map<ResponseAsignaturaGestorMapeoModel, AsignaturaIntegrationGestorMapeoModel>(asignatura);
        }

        public virtual async Task<EstudioIntegrationGestorMapeoModel> GetEstudio(int idEstudioGestor)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MapeoEstudioApi}/{idEstudioGestor}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idEstudioGestor.ToString());
                _logger.LogError(mensajeError);
                return null;
            }

            var estudio = await response.Content.ReadAsAsync<ResponseEstudioGestorMapeoModel>();
            return _mapper.Map<ResponseEstudioGestorMapeoModel, EstudioIntegrationGestorMapeoModel>(estudio);
        }
    }
}
