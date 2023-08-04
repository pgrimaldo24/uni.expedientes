using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.Evaluaciones;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class EvaluacionesServiceClient : IEvaluacionesServiceClient
    {
        private const string NivelesAsociadosEscalasApi = "api/v1/niveles-asociados-escalas";

        private readonly HttpClient _client;
        private readonly ILogger<ErpAcademicoServiceClient> _logger;
        private readonly IMapper _mapper;

        public EvaluacionesServiceClient(IHttpClientFactory clientFactory,
            ILogger<ErpAcademicoServiceClient> logger,
            IOptions<ServicesClientsSettings> clientSettings)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.EvaluacionServices.Host);
            _client.DefaultRequestHeaders.Add("X-Api-Key", clientSettings.Value.EvaluacionServices.ApiKey);
            _logger = logger;
            _mapper = ConfigureMapping();
        }

        private static Mapper ConfigureMapping()
        {
            return new(new MapperConfiguration(conf =>
            {
                 conf.CreateMap<ResponseActa, ActaModel>();
                 conf.CreateMap<ResponseCalificacion, CalificacionModel>();
                 conf.CreateMap<ResponseCalificacionList, CalificacionListModel>();
                 conf.CreateMap<ResponseCategoriaEvaluacion, CategoriaEvaluacionModel>();
                 conf.CreateMap<ResponseConfiguracion, ConfiguracionModel>();
                 conf.CreateMap<ResponseConfiguracionVersionEscala, ConfiguracionVersionEscalaModel>();
                 conf.CreateMap<ResponseEscala, EscalaModel>();
                 conf.CreateMap<ResponseEvaluacion, EvaluacionModel>();
                 conf.CreateMap<ResponseNivelAsociadoEscala, NivelAsociadoEscalaModel>();

            }));
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio Evaluaciones : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
            ticks,
            response.RequestMessage.RequestUri,
            response.RequestMessage.Method,
            response.RequestMessage.Headers,
            body,
            await response.Content.ReadAsStringAsync(),
            response.StatusCode);
            return mensajeError;
        }

        public virtual async Task<ConfiguracionVersionEscalaModel> GetConfiguracionEscalaFromNivelesAsociadosEscalas(
            int? idAsignaturaOfertada, int? idAsignaturaPlan)
        {
            var endPoint = $"{NivelesAsociadosEscalasApi}/configuracion-escala";
            var parameters = new
            {
                where = new List<object>()
            };
            if (idAsignaturaOfertada.HasValue)
            {
                parameters.where.Add(new
                {
                    field = "idAsignaturaOfertada",
                    value = idAsignaturaOfertada
                });
            }
            if (idAsignaturaPlan.HasValue)
            {
                parameters.where.Add(new
                {
                    field = "idAsignaturaPlan",
                    value = idAsignaturaPlan
                });
            }
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"{idAsignaturaOfertada} - {idAsignaturaPlan}");
                _logger.LogError(mensajeError);
                return null;
            }

            var configuracionVersionEscala = await response.Content.ReadAsAsync<ResponseConfiguracionVersionEscala>();
            return _mapper.Map<ResponseConfiguracionVersionEscala, ConfiguracionVersionEscalaModel > (configuracionVersionEscala);
        }
    }
}
