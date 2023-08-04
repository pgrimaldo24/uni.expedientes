using AutoMapper;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class ExpedientesGestorUnirServiceClient : IExpedientesGestorUnirServiceClient
    {
        private const string GenericError =
            "Expedientes Gestor Unir [{0}]: {1}";
        private const string ExpedientesApi = "Expedientes/api/v1/expedientes";
        private const string AsignaturasComercialApi = "Expedientes/api/v1/asignaturas/formato/erp-comercial";
        private const string AsignaturasAcademicoCompletoApi = "Expedientes/api/v1/asignaturas/formato/erp-academico-completo";
        private const string ReconocimientosApi = "Reconocimientos/api/v1/reconocimientos";

        private readonly HttpClient _client;
        private readonly ISecurityIntegrationServices _securityCrosscuttingService;
        private readonly ILogger<ExpedientesGestorUnirServiceClient> _logger;
        private readonly IStringLocalizer<ExpedientesGestorUnirServiceClient> _localizer;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public ExpedientesGestorUnirServiceClient(IHttpClientFactory clientFactory,
            IOptions<ServicesClientsSettings> clientSettings, ISecurityIntegrationServices securityCrosscuttingService,
            ILogger<ExpedientesGestorUnirServiceClient> logger,
            IStringLocalizer<ExpedientesGestorUnirServiceClient> localizer, IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.ExpedientesGestorUnirServices.Host);

            _securityCrosscuttingService = securityCrosscuttingService;
            _logger = logger;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _mapper = ConfigureMapping();
        }

        private static Mapper ConfigureMapping()
        {
            return new(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseExpedienteBloqueoGestor, ExpedienteBloqueoModel>();
                conf.CreateMap<ResponseExpedienteErpComercialIntegrationModel, ExpedienteErpComercialIntegrationModel>();
                conf.CreateMap<ResponseAsignaturaErpComercialExpedientesIntegrationModel, AsignaturaErpComercialExpedientesIntegrationModel>();
                conf.CreateMap<ResponseReconocimientosOrigenModel, ReconocimientosOrigenModel>();
                conf.CreateMap<ResponseBloqueosIntegrationModel, BloqueosIntegrationModel>();
                conf.CreateMap<ResponseItinerariosFinalizadosIntegrationModel, ItinerariosFinalizadosIntegrationModel>();
                conf.CreateMap<ResponsePracticaIntegrationModel, PracticaIntegrationModel>();
                conf.CreateMap<ResponseTipoItinerarioIntegrationModel, TipoItinerarioIntegrationModel>();
                conf.CreateMap<ResponseViasAccesoIntegrationModel, ViasAccesoIntegrationModel>();
                conf.CreateMap<ResponseIdiomaAcreditacionIntegrationModel, IdiomaAcreditacionIntegrationModel>();
                conf.CreateMap<ResponseExpedienteExpedientesIntegrationModel, ExpedienteExpedientesIntegrationModel>();
                conf.CreateMap<ResponseAsignaturaErpAcademicoExpedientesIntegrationModel, AsignaturaErpAcademicoExpedientesIntegrationModel>();
                conf.CreateMap<ResponseMotivoBloqueado, MotivoBloqueado>();
                conf.CreateMap<ResponseAccionBloqueada, AccionBloqueada>();
                conf.CreateMap<ResponseReconocimientoServicioGestorUnir, ReconocimientoIntegrationGestorModel>();
                conf.CreateMap<ResponseReconocimientoGestorUnir, ReconocimientoGestorModel>();
                conf.CreateMap<ResponseTransversalGestorUnir, TransversalGestorModel>();
                conf.CreateMap<ResponseAsignaturaGestorUnir, AsignaturaGestorModel>();
                conf.CreateMap<ResponseSeminarioGestorUnir, SeminarioGestorModel>();
                conf.CreateMap<ResponseExtensionUniversitariaGestorUnir, ExtensionUniversitariaGestorModel>();
                conf.CreateMap<ResponseReconocimientoCommonGestorUnir, ReconocimientoCommonGestorModel>();
            }));
        }

        public virtual async Task<ExpedienteBloqueoModel> GetBloqueoExpediente(int idAlumno, int? idPlan = null)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ExpedientesApi}/bloqueos?idAlumno={idAlumno}&idPlanErp={idPlan}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                throw new BadRequestException(_localizer[string.Format(GenericError, ticks, $"No se ha podido obtener el Bloqueo del Expediente")]);
            }

            var bloqueoExpediente = await response.Content.ReadAsAsync<ResponseExpedienteBloqueoGestor>();
            return _mapper.Map<ResponseExpedienteBloqueoGestor, ExpedienteBloqueoModel>(bloqueoExpediente);
        }

        public virtual async Task<ExpedienteErpComercialIntegrationModel> GetExpedienteGestorFormatoComercialWithAsignaturasErp(string idIntegracionAlumno, int idPlan)
        {
            await SetAuthorizationClient();
            var endPoint = $"{AsignaturasComercialApi}?idAlumno={idIntegracionAlumno}&idPlanErp={idPlan}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                throw new BadRequestException(_localizer[string.Format(GenericError, ticks, "No se ha podido obtener el Expediente desde Gestor Unir")]);
            }

            var resultResponse = await response.Content.ReadAsAsync<ResponseExpedienteErpComercialIntegrationModel>();
            var expediente = _mapper.Map<ResponseExpedienteErpComercialIntegrationModel, ExpedienteErpComercialIntegrationModel>(resultResponse);
            if (expediente.Asignaturas == null || !expediente.Asignaturas.Any())
                return expediente;

            var parametersAsignaturas = new AsignaturaPlanListParameters
            {
                NoPaged = true,
                IdPlan = idPlan
            };
            var resultAsignaturasPlanes = await _erpAcademicoServiceClient.GetAsignaturasPlanes(parametersAsignaturas);
            var idsAsignaturasPlanes = resultAsignaturasPlanes.Select(ap => ap.Id).Distinct().ToList();
            if (idsAsignaturasPlanes.Any())
            {
                expediente.Asignaturas = expediente.Asignaturas
                    .Where(a => idsAsignaturasPlanes.Contains(a.IdAsignatura)).ToList();
            }

            return expediente;
        }

        public virtual async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErpWithAsignaturas(
            string idIntegracionAlumno, int idPlan)
        {
            var result = await GetExpedienteGestorFormatoErp(idIntegracionAlumno, idPlan);
            if (result.HasErrors) return result;

            var expediente = result.Value;

            var parametersAsignaturas = new AsignaturaPlanListParameters
            {
                NoPaged = true,
                IdPlan = idPlan
            };
            var resultAsignaturasPlanes = await _erpAcademicoServiceClient.GetAsignaturasPlanes(parametersAsignaturas);
            var idsAsignaturasPlanes = resultAsignaturasPlanes.Select(ap => ap.Id).Distinct().ToList();
            if (!idsAsignaturasPlanes.Any())
                return result;

            var asignaturasReconocidas = expediente.Asignaturas.Where(a => a.GetAsignaturasReconocidas()).ToList();
            expediente.Asignaturas = expediente.Asignaturas
                .Where(a => idsAsignaturasPlanes.Contains(a.IdAsignatura)).ToList();
            if (asignaturasReconocidas.Any())
            {
                expediente.Asignaturas.AddRange(asignaturasReconocidas);
            }

            return result;
        }

        public virtual async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErp(
            string idIntegracionAlumno, int idPlan)
        {
            await SetAuthorizationClient();
            var endPoint = $"{AsignaturasAcademicoCompletoApi}?idAlumno={idIntegracionAlumno}&idPlanErp={idPlan}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                if (response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    return JsonSerializer.Deserialize<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>>(
                        contentError);
                }

                throw new BadRequestException(
                    _localizer[$"[Expedientes Gestor ({ticks})]: Ha ocurrido un error no controlado."]);
            }

            var resultResponse = await response.Content.ReadAsAsync<ResponseExpedienteExpedientesIntegrationModel>();
            var expediente =
                _mapper.Map<ResponseExpedienteExpedientesIntegrationModel, ExpedienteExpedientesIntegrationModel>(
                    resultResponse);
            var result = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = expediente
            };
            
            return result;
        }

        protected internal virtual async Task SetAuthorizationClient()
        {
            var resultToken = await _securityCrosscuttingService.GetTokenServiciosDesUnirExpedientes();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultToken);
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio de Gestor Unir : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
            ticks,
            response.RequestMessage.RequestUri,
            response.RequestMessage.Method,
            response.RequestMessage.Headers,
            body,
            await response.Content.ReadAsStringAsync(),
            response.StatusCode);
            return mensajeError;
        }

        public virtual async Task<ReconocimientoIntegrationGestorModel> GetReconocimientos(
            string idIntegracionAlumno, int idEstudio)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ReconocimientosApi}/alumno/{idIntegracionAlumno}/estudio/{idEstudio}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"{idIntegracionAlumno} - {idEstudio}");
                _logger.LogError(mensajeError);
                return null;
            }

            var reconocimientos = await response.Content.ReadAsAsync<ResponseReconocimientoServicioGestorUnir>();
            return _mapper.Map<ResponseReconocimientoServicioGestorUnir, 
                ReconocimientoIntegrationGestorModel>(reconocimientos);
        }
        
    }
}
