using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedicionTitulos;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class ExpedicionTitulosServiceClient : IExpedicionTitulosServiceClient
    {
        private const string SolicitudesApi = "api/v1/solicitudes";
        private const string TipoSolicitudesApi = "api/v1/tipos-solicitudes";

        private readonly HttpClient _client;
        private readonly ILogger<ErpAcademicoServiceClient> _logger;
        private readonly IMapper _mapper;

        public ExpedicionTitulosServiceClient(IHttpClientFactory clientFactory,
            ILogger<ErpAcademicoServiceClient> logger,
            IOptions<ServicesClientsSettings> clientSettings)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.ExpedicionTitulosServices.Host);
            _client.DefaultRequestHeaders.Add("X-Api-Key", clientSettings.Value.ExpedicionTitulosServices.ApiKey);
            _logger = logger;
            _mapper = ConfigureMapping();
        }

        private static Mapper ConfigureMapping()
        {
            return new(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseSolicitudExpedicionTitulos, SolicitudExpedicionTitulosModel>();
                conf.CreateMap<ResponseEstadoExpedicionTitulos, EstadoExpedicionTitulosModel>();
                conf.CreateMap<ResponseTipoExpedicionTitulos, TipoExpedicionTitulosModel>();
                conf.CreateMap<ResponseUniversidadExpedicionTitulos, UniversidadExpedicionTitulosModel>();
                conf.CreateMap<ResponseTiposSolicitudesTitulos, TiposSolicitudesTitulosModel>();
                conf.CreateMap<ResponseTipoSolicitudTitulo, TipoSolicitudTituloModel>();
            }));
        }

        public virtual async Task<List<SolicitudExpedicionTitulosModel>> GetSolicitudes(int idIntegracionAlumno, int idPlan, bool excluirCanceladas)
        {
            var endPoint = $"{SolicitudesApi}/estados?idIntegracionAlumno={idIntegracionAlumno}&idPlan={idPlan}&excluirCanceladas={excluirCanceladas}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<SolicitudExpedicionTitulosModel>();
            }

            var solicitudes = await response.Content.ReadAsAsync<List<ResponseSolicitudExpedicionTitulos>>();
            return _mapper.Map<List<ResponseSolicitudExpedicionTitulos>, List<SolicitudExpedicionTitulosModel>>(solicitudes);
        }

        public virtual async Task<List<TiposSolicitudesTitulosModel>> GetTiposSolicitudes(TiposSolicitudesParameters tiposSolicitudesParameters)
        {
            var endPoint = $"{TipoSolicitudesApi}/query-configuraciones";
            var parameters = new
            {
                where = new List<object>()
            };
            if (tiposSolicitudesParameters.IdsRefUniversidad.Any())
            {
                parameters.where.Add(new
                {
                    field = "idsRefUniversidades",
                    value = tiposSolicitudesParameters.IdsRefUniversidad
                });
            }

            if (!string.IsNullOrWhiteSpace(tiposSolicitudesParameters.FilterDisplayName))
            {
                parameters.where.Add(new
                {
                    field = "filterDisplayName",
                    value = tiposSolicitudesParameters.FilterDisplayName
                });
            }

            if (tiposSolicitudesParameters.ConFechaPago is not null)
            {
                parameters.where.Add(new
                {
                    field = "conFechaPago",
                    value = tiposSolicitudesParameters.ConFechaPago.Value
                });
            }

            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"IdsRefUnivesidad : {tiposSolicitudesParameters.IdsRefUniversidad}");
                _logger.LogError(mensajeError);
                return null;
            }
            var tiposSolicitudesTitulos = await response.Content.ReadAsAsync<List<ResponseTiposSolicitudesTitulos>>();
            return _mapper.Map<List<ResponseTiposSolicitudesTitulos>, List<TiposSolicitudesTitulosModel>>(tiposSolicitudesTitulos);
        }

        public virtual async Task<TipoSolicitudTituloModel> GetTipoSolicitudTituloById(int id)
        {
            var endPoint = $"{TipoSolicitudesApi}/{id}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"Id : {id}");
                _logger.LogError(mensajeError);
                return null;
            }

            var solicitudes = await response.Content.ReadAsAsync<ResponseTipoSolicitudTitulo>();
            return _mapper.Map<ResponseTipoSolicitudTitulo, TipoSolicitudTituloModel>(solicitudes);
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio Expedición de Títulos : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
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
