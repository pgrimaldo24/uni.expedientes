using AutoMapper;
using Infrastructure.ServicesClients.Models.Financiero;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Financiero;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class FinancieroServiceClient : IFinancieroServiceClient
    {
        private const string DEUDA_CLIENTE_API = "api/v1/deuda-cliente";

        private readonly HttpClient _client;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancieroServiceClient> _logger;

        public FinancieroServiceClient(IHttpClientFactory clientFactory,
            IOptions<ServicesClientsSettings> clientSettings, ILogger<FinancieroServiceClient> logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.FinancieroServices.Host);
            _client.DefaultRequestHeaders.Add("X-Api-Key", clientSettings.Value.FinancieroServices.ApiKey);
            _mapper = ConfigureMapping();
        }

        protected static Mapper ConfigureMapping()
        {
            return new(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseDeudaClienteModel, DeudaClienteModel>();
                conf.CreateMap<ResponseFacturaModel, FacturaModel>();
                conf.CreateMap<ResponseReciboModel, ReciboModel>();
            }));
        }

        public async Task<DeudaClienteModel> GetDeudaCliente(DeudaClienteParameters request)
        {
            var endPoint = $"{DEUDA_CLIENTE_API}";
            var parameters = new
            {
                codigoUniversidad = request.IdIntegracionUniversidad,
                idGestor = request.IdIntegracionAlumno
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new DeudaClienteModel();
            }

            var deudaCliente = await response.Content.ReadAsAsync<ResponseDeudaClienteModel>();
            return _mapper
                .Map<ResponseDeudaClienteModel, DeudaClienteModel>(deudaCliente);
        }
    }
}
