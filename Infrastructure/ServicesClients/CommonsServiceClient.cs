
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Commons;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.Commons;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class CommonsServiceClient : ICommonsServiceClient
    {
        private const string PrefixUniversities = "api/v1/universities";
        private const string PrefixEducationalInstitutions = "api/v1/educational-institutions";
        private const string PrefixCountries = "api/v1/countries";

        private readonly HttpClient _client;
        private readonly ILogger<CommonsServiceClient> _logger;
        private readonly IMapper _mapper;

        public CommonsServiceClient(IHttpClientFactory clientFactory, ILogger<CommonsServiceClient> logger, IOptions<ServicesClientsSettings> clientSettings)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.CommonsServices.Host);
            _logger = logger;
            _mapper = ConfigureMapping();
        }

        protected Mapper ConfigureMapping()
        {
            return new Mapper(new MapperConfiguration(conf =>
            {
                conf.CreateMap<ResponseTipoDocumentoIdentidadCommons, TipoDocumentoIdentidadCommonsModel>();
                conf.CreateMap<ResponseUniversitiesBindingTypeCommons, UniversitiesBindingTypeCommonsModel>();
                conf.CreateMap<ResponseEducationalInstitutionCommon, EducationalInstitutionsCommonModel>();
                conf.CreateMap<ResponseCountryCommon, CountryCommonsModel>();
                conf.CreateMap<ResponseDivisionCommon, DivisionCommonsModel>();
                conf.CreateMap<ResponseLevelDivisionCommon, LevelDivisionCommonsModel>();
                conf.CreateMap<ResponseUniversitiesCommons, UniversitiesModel>();
            }));
        }

        public async Task<UniversitiesBindingTypeCommonsModel[]> GetStudentUniversitiesBindingTypes(string code)
        {
            var endPoint = $"{PrefixUniversities}/{code}/student-university-binding-types";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return Array.Empty<UniversitiesBindingTypeCommonsModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<ResponseUniversitiesBindingTypeCommons[]>();
            return _mapper.Map<ResponseUniversitiesBindingTypeCommons[], UniversitiesBindingTypeCommonsModel[]>(
                resultService);
        }

        public async Task<List<EducationalInstitutionsCommonModel>> GetEducationalInstitutions(EducationalInstitutionsFiltersParameters parameters)
        {
            var endPoint = $"{PrefixEducationalInstitutions}?indexpage={parameters.IndexPage}&limit={parameters.Limit}&name={parameters.Nombre}&countryCode={parameters.CodeCountry}";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<EducationalInstitutionsCommonModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<List<ResponseEducationalInstitutionCommon>>();
            return _mapper.Map<List<ResponseEducationalInstitutionCommon>, List<EducationalInstitutionsCommonModel>>(
                resultService);
        }


        public async Task<List<CountryCommonsModel>> GetCountries(CountryParameters parameters)
        {
            var endPoint = $"{PrefixCountries}?index={parameters.Offset}&count={parameters.Limit}&name={parameters.Nombre}";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<CountryCommonsModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<List<ResponseCountryCommon>>();
            return _mapper.Map<List<ResponseCountryCommon>, List<CountryCommonsModel>>(
                resultService);
        }

        public async Task<CountryCommonsModel> GetCountry(string countryCode)
        {
            var endPoint = $"{PrefixCountries}/{countryCode}";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new CountryCommonsModel();
            }

            var resultService =
                await response.Content.ReadAsAsync<ResponseCountryCommon>();
            return _mapper.Map<ResponseCountryCommon, CountryCommonsModel>(resultService);
        }

        public async Task<List<DivisionCommonsModel>> GetDivisionsCountries(string isoCode)
        {
            var endPoint = $"{PrefixCountries}/{isoCode}/divisions";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<DivisionCommonsModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<List<ResponseDivisionCommon>>();
            return _mapper.Map<List<ResponseDivisionCommon>, List<DivisionCommonsModel>>(
                resultService);
        }

        public async Task<List<LevelDivisionCommonsModel>> GetDivisionCountryServiciosGestorUnir(DivisionParameters parameters)
        {
            var endPoint =
                $"{PrefixCountries}/{parameters.IsoCode}/divisions/{parameters.LevelCode}/entities?superEntityCode={parameters.SuperEntityCode}&name={parameters.Name}&index={parameters.Offset}&count={parameters.Limit}";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<LevelDivisionCommonsModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<List<ResponseLevelDivisionCommon>>();
            return _mapper.Map<List<ResponseLevelDivisionCommon>, List<LevelDivisionCommonsModel>>(
                resultService);
        }

        public async Task<List<LevelDivisionCommonsModel>> GetPathDivision(string code)
        {
            var endPoint = $"api/v1/entities/{code}/path";

            var response = await _client.GetAsync(endPoint);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new List<LevelDivisionCommonsModel>();
            }

            var resultService =
                await response.Content.ReadAsAsync<List<ResponseLevelDivisionCommon>>();
            return _mapper.Map<List<ResponseLevelDivisionCommon>, List<LevelDivisionCommonsModel>>(
                resultService);
        }

        public async Task<UniversitiesModel> GetUniversities(string code)
        {
            var endPoint = $"{PrefixUniversities}/{code}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return new UniversitiesModel();
            }

            var resultService =
                await response.Content.ReadAsAsync<ResponseUniversitiesCommons>();
            return _mapper.Map<ResponseUniversitiesCommons, UniversitiesModel>(
                resultService);
        }
    }
}
