using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.FilesTypes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Personal;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Security;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.FilesTypes;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Personal;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Infrastructure.ServicesClients
{
    public class ErpAcademicoServiceClient : IErpAcademicoServiceClient
    {
        private const string PlanesApi = "api/v1/planes";
        private const string ExpedientesApi = "api/v1/expedientes";
        private const string AlumnosApi = "api/v1/alumnos";
        private const string PersonasApi = "api/v1/personas";
        private const string TrabajadoresApi = "api/v1/trabajadores";
        private const string NodosApi = "api/v1/nodos";
        private const string VersionesPlanesApi = "api/v1/versiones-planes";
        private const string ViasAccesoApi = "api/v1/vias-acceso";
        private const string AsignaturasPlanesApi = "api/v1/asignaturas-planes";
        private const string SecurityApi = "api/v1/security-service";
        private const string UniversidadesApi = "api/v1/universidades";
        private const string EspecializacionesApi = "api/v1/especializaciones";
        private const string ViasAccesosPlanesApi = "api/v1/vias-accesos-planes";
        private const string MatriculasApi = "api/v1/matriculas";
        private const string ModosRequerimientoDocumentacionApi = "api/v1/modos-requerimiento-documentacion";
        private const string FilesTypesApi = "api/v1/files-types";
        private const string TiposEstudiosApi = "api/v1/tipos-estudios";
        private const string EstudiosApi = "api/v1/estudios";
        private const string TiposAsignaturasApi = "api/v1/tipos-asignatura";
        private const string IdiomasApi = "api/v1/idiomas";
        private const string RequerimientosPlanesApi = "api/v1/requerimientos-planes";
        private const string CausasBajasMatriculaApi = "api/v1/causas-bajas-matriculas";
        private const string TipoBajasMatriculaApi = "api/v1/tipos-bajas-matriculas";
        private const string AsignaturasMatriculadasApi = "api/v1/asignaturas-matriculadas";
        private const string PublicAsignaturasOfertadasApi = "public/api/v1/asignaturas-ofertadas";

        private readonly HttpClient _client;
        private readonly ILogger<ErpAcademicoServiceClient> _logger;
        private readonly IMapper _mapper;
        private readonly ISecurityIntegrationServices _securityCrosscuttingService;
        private readonly IStringLocalizer<ErpAcademicoServiceClient> _localizer;

        public ErpAcademicoServiceClient(
            IHttpClientFactory clientFactory,
            ILogger<ErpAcademicoServiceClient> logger,
            ISecurityIntegrationServices securityCrosscuttingService,
            IOptions<ServicesClientsSettings> clientSettings, IStringLocalizer<ErpAcademicoServiceClient> localizer)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(clientSettings.Value.ErpAcademicoServices.Host);

            _logger = logger;
            _securityCrosscuttingService = securityCrosscuttingService;
            _localizer = localizer;
            _mapper = ConfigureMapping();
        }

        private static Mapper ConfigureMapping()
        {
            return new(new MapperConfiguration(conf =>
            {
                ConfigureMappingGlobal(conf);
                ConfigureMappingExpedientes(conf);
                ConfigureMappingPlanesEstudio(conf);
                ConfigureMappingTitulos(conf);
                ConfigureMappingMatriculacion(conf);
                ConfigureMappingPersonal(conf);
            }));
        }
        private static void ConfigureMappingGlobal(IProfileExpression conf)
        {
            conf.CreateMap<ResponseUniversidadErpAcademico, UniversidadAcademicoModel>();
            conf.CreateMap<ResponseCentroErpAcademico, CentroAcademicoModel>();
            conf.CreateMap<ResponseAreaAcademicaErpAcademico, AreaAcademicaAcademicoModel>();
            conf.CreateMap<ResponsePersonaErpAcademico, PersonaAcademicoModel>();
            conf.CreateMap<ResponseModoRequerimientoDocumentacionErpAcademico, ModoRequerimientoDocumentacionAcademicoModel>();
            conf.CreateMap<FileTypeErpAcademico, FileTypeAcademicoModel>();
            conf.CreateMap<ResponseIdiomaErpAcademico, IdiomaAcademicoModel>();
        }

        private static void ConfigureMappingExpedientes(IProfileExpression conf)
        {
            conf.CreateMap<ResponseExpedienteErpAcademico, ExpedienteAcademicoModel>();
            conf.CreateMap<ResponseTitulacionAccesoErpAcademico, TitulacionAccesoAcademicoModel>();
            conf.CreateMap<ResponseValidateAlumnoMatriculacionErpAcademico, ValidateAlumnoMatriculacionErpAcademicoModel>();
            conf.CreateMap<ResponseDocumentoAlumnoErpAcademico, DocumentoAlumnoModel>();
            conf.CreateMap<ResponseDocumentoAcademicoErpAcademico, DocumentoAcademicoModel>();
            conf.CreateMap<ResponsePeriodoAcademicoMatriculaErpAcademico, PeriodoAcademicoMatriculaModel>();
            conf.CreateMap<ResponseMatriculaAcademico, MatriculaModel>();
            conf.CreateMap<ResponseCondicionConsolidadaErpAcademico, CondicionConsolidadaModel>();
            conf.CreateMap<ResponseCondicionMatriculaErpAcademico, CondicionMatriculaModel>();
        }

        private static void ConfigureMappingPlanesEstudio(IProfileExpression conf)
        {
            conf.CreateMap<ResponseEstudioErpAcademico, EstudioAcademicoModel>();
            conf.CreateMap<ResponsePlanErpAcademico, PlanAcademicoModel>();
            conf.CreateMap<ResponseTipoEstudioErpAcademico, TipoEstudioAcademicoModel>();
            conf.CreateMap<ResponseHitoErpAcademico, HitoErpAcademicoModel>();
            conf.CreateMap<ResponseNodoErpAcademico, NodoErpAcademicoModel>();
            conf.CreateMap<ResponseTipoNodoErpAcademico, TipoNodoAcademicoModel>();
            conf.CreateMap<ResponseVersionPlanErpAcademico, VersionPlanAcademicoModel>();
            conf.CreateMap<ResponseViaAccesoErpAcademico, ViaAccesoAcademicoModel>();
            conf.CreateMap<ResponseViaAccesoPlanErpAcademico, ViaAccesoPlanAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaPlanErpAcademico, AsignaturaPlanErpAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaErpAcademico, AsignaturaErpAcademicoModel>();
            conf.CreateMap<ResponseTipoAsignaturaErpAcademico, TipoAsignaturaErpAcademicoModel>();
            conf.CreateMap<ResponsePlanSuperadoErpAcademico, PlanSuperadoErpAcademicoModel>();
            conf.CreateMap<ResponseElementoSuperadoErpAcademico, ElementoSuperadoErpAcademicoModel>();
            conf.CreateMap<ResponseTrayectoPlanErpAcademico, TrayectoPlanErpAcademicoModel>();
            conf.CreateMap<ResponseRequerimientoPlanErpAcademico, RequerimientoPlanErpAcademicoModel>();
            conf.CreateMap<ResponseRequerimientoTipoAsignaturaErpAcademico, RequerimientoTipoAsignaturaErpAcademicoModel>();
            conf.CreateMap<ResponseDatosGestorErpAcademico, DatosGestorErpAcademicoModel>();
            conf.CreateMap<ResponseReconocimientosOrigenModel, ReconocimientosOrigenErpAcademicoModel>();
            conf.CreateMap<ResponseBloqueSuperadoErpAcademico, BloqueSuperadoErpAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaPlanBloqueErpAcademico, AsignaturaPlanBloqueErpAcademicoModel>();
            conf.CreateMap<ResponseSubBloqueErpAcademico, SubBloqueErpAcademicoModel>();
            conf.CreateMap<ResponseTipoSubBloqueErpAcademico, TipoSubBloqueErpAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaPlanSubBloqueErpAcademico, AsignaturaPlanSubBloqueErpAcademicoModel>();
            conf.CreateMap<ResponseArcoErpAcademico, ArcoErpAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaPlanTitulacionErpAcademico, AsignaturaPlanTitulacionErpAcademico>();
            conf.CreateMap<ResponseClasificacionViaAcceso, ClasificacionViaAccesoModel>();
            conf.CreateMap<ResponseEspecializacionIntegrationModel, EspecializacionIntegrationModel>();
            conf.CreateMap<ResponseRequerimientoPlanAcademico, RequerimientoPlanAcademicoModel>();
            conf.CreateMap<ResponseTipoAsignaturaRequerimiento, TipoAsignaturaRequerimientoModel>();
            conf.CreateMap<ResponseTipoAsignaturaExpediente, TipoAsignaturaExpedienteModel>();
            conf.CreateMap<ResponseTrayectoPlan, TrayectoPlanModel>();
            conf.CreateMap<ResponseNodoFinal, NodoFinalModel>();
            conf.CreateMap<ResponseNodoInicial, NodoInicialModel>();
            conf.CreateMap<ResponseHito, HitoModel>();
            conf.CreateMap<ResponseHitoEspecializacion, HitoEspecializacionModel>();
            conf.CreateMap<ResponseEspecializacion, EspecializacionModel>();
            conf.CreateMap<ResponseHitoTitulo, HitoTituloModel>();
            conf.CreateMap<ResponseTitulo, TituloModel>();
            conf.CreateMap<ResponseGrafoDataErpAcademico, GrafoDataAcademicoModel>();
            conf.CreateMap<ResponseGrafoErpAcademico, GrafoAcademicoModel>();
            conf.CreateMap<ResponseNodoAcademico, NodoAcademicoModel>();
            conf.CreateMap<ResponseArcoAcademico, ArcoAcademicoModel>();
            conf.CreateMap<ResponseBloqueAcademico, BloqueAcademicoModel>();
            conf.CreateMap<ResponseNodoDestinoAcademico, NodoDestinoAcademicoModel>();
            conf.CreateMap<ResponsePlanEstudioErpAcademico, PlanEstudioErpAcademicoModel>();
        }

        private static void ConfigureMappingTitulos(IProfileExpression conf)
        {
            conf.CreateMap<ResponseEspecializacionErpAcademico, EspecializacionAcademicoModel>();
            conf.CreateMap<ResponseTipoEspecializacionErpAcademico, TipoEspecializacionAcademicoModel>();
            conf.CreateMap<ResponseTituloErpAcademico, TituloAcademicoModel>();
        }

        private static void ConfigureMappingMatriculacion(IProfileExpression conf)
        {
            conf.CreateMap<ResponseAlumnoErpAcademico, AlumnoAcademicoModel>();
            conf.CreateMap<ResponsePersonaAcademico, PersonaErpAcademicoModel>();
            conf.CreateMap<ResponseMatriculaErpAcademico, MatriculaErpAcademicoModel>();
            conf.CreateMap<ResponseEstadoMatriculaErpAcademico, EstadoMatriculaErpAcademicoModel>();
            conf.CreateMap<ResponseDocumentoIdentificacionAcademico, DocumentoIdentificacionModel>();
            conf.CreateMap<ResponseMatriculaAcademicoModel, MatriculaAcademicoModel>();
            conf.CreateMap<ResponseDocumentoAlumnoErpAcademicoModel, DocumentoAlumnoAcademicoModel>();
            conf.CreateMap<ResponseTipoMatriculaAcademicoModel, TipoMatriculaAcademicoModel>();
            conf.CreateMap<ResponsePlanOfertadoDtoAcademicoModel, PlanOfertadoDtoAcademicoModel>();
            conf.CreateMap<ResponseRegionEstudioAcademicoModel, RegionEstudioAcademicoModel>();
            conf.CreateMap<ResponseEstadoMatriculaAcademicoModel, EstadoMatriculaAcademicoModel>();
            conf.CreateMap<ResponsePeriodoAcademicoAcademicoModel, PeriodoAcademicoAcademicoModel>();
            conf.CreateMap<ResponseAnyoAcademicoAcademicoModel, AnyoAcademicoAcademicoModel>();
            conf.CreateMap<ResponsePlanErpAcademico, PlanAcademicoModel>();
            conf.CreateMap<ResponseAsignaturaMatricula, AsignaturaMatriculadaModel>();
            conf.CreateMap<ResponseCausaBajaMatricula, CausaBajaMatriculaModel>();
            conf.CreateMap<ResponseTipoBajaMatriculaErpAcademico, TipoBajaMatriculaModel>();
        }

        private static void ConfigureMappingPersonal(IProfileExpression conf)
        {
            conf.CreateMap<ResponseTrabajadorErpAcademico, TrabajadorAcademicoModel>();
        }

        #region Expedientes Erp

        public virtual async Task<ExpedienteAcademicoModel[]> GetExpedientesAsync(string[] idsIntegracion,
            CancellationToken cancellationToken)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ExpedientesApi}/advanced-search";
            var parameters = new
            {
                NoPaged = true,
                FilterIdsIntegracion = idsIntegracion
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(parameters));
                _logger.LogError(mensajeError);
                return null;
            }

            var expedientes = await response.Content.ReadAsAsync<ResponseExpedienteErpAcademico[]>(cancellationToken);
            return _mapper.Map<ResponseExpedienteErpAcademico[], ExpedienteAcademicoModel[]>(expedientes);
        }

        public virtual async Task<ExpedienteAcademicoModel> GetExpediente(int id)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ExpedientesApi}/integracion/{id}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"[{ticks}] Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                throw new BadRequestException(
                    _localizer[$"[{ticks}] ERP Académico: No se ha podido obtener el expediente"]);
            }

            var expediente = await response.Content.ReadAsAsync<ResponseExpedienteErpAcademico>();
            return _mapper.Map<ResponseExpedienteErpAcademico, ExpedienteAcademicoModel>(expediente);
        }

        public virtual async Task<Unit> ModifyExpedienteByIdIntegracion(string idIntegracion,
            ExpedienteEditParameters parameters)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ExpedientesApi}/integracion/{idIntegracion}";
            var contentRequest =
                new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, contentRequest);
            if (response.IsSuccessStatusCode)
                return Unit.Value;

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
            var resultValidation = JsonSerializer.Deserialize<ResultConfirmation>(content);
            throw new ValidationErrorsException(resultValidation.Errors.ToArray());
        }

        #endregion

        public async Task<List<AlumnoAcademicoModel>> GetAlumnos(AlumnoListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new AlumnoAdvancedSearchParameters
            {
                FilterNombreCompleto = parameters.Search,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "id",
                OrderDirection = "asc"
            };

            var endPoint = $"{AlumnosApi}/advanced-search";
            var content =
                new ObjectContent<AlumnoAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<AlumnoAcademicoModel>();
            }

            var contentAlumnos = await response.Content.ReadAsAsync<ResponseContentAlumnosAdvancedSearchErpAcademico>();
            return contentAlumnos.Content.Elements
                .Select(u => _mapper.Map<ResponseAlumnoErpAcademico, AlumnoAcademicoModel>(u))
                .ToList();
        }

        public async Task<PersonaAcademicoModel[]> GetPersonas(string[] userIds)
        {
            await SetAuthorizationClient();
            var endPoint = $"{PersonasApi}/advanced-search";
            var parameters = new
            {
                FilterIdsSeguridad = userIds,
                NoPaged = true
            };
            var content = new StringContent(JsonSerializer.Serialize(parameters), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return Array.Empty<PersonaAcademicoModel>();
            }

            var personas = await response.Content.ReadAsAsync<ResponsePersonaErpAcademico[]>();
            return _mapper.Map<ResponsePersonaErpAcademico[], PersonaAcademicoModel[]>(personas);
        }

        public async Task<List<TrabajadorAcademicoModel>> GetTrabajadores(TrabajadorListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new TrabajadorAdvancedSearchParameters
            {
                FilterNombreCompleto = parameters.Search,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            var endPoint = $"{TrabajadoresApi}/advanced-search";
            var content =
                new ObjectContent<TrabajadorAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<TrabajadorAcademicoModel>();
            }

            var trabajadores = await response.Content.ReadAsAsync<List<ResponseTrabajadorErpAcademico>>();
            return _mapper.Map<List<ResponseTrabajadorErpAcademico>, List<TrabajadorAcademicoModel>>(trabajadores);
        }

        public virtual async Task<NodoErpAcademicoModel> GetNodo(int id)
        {
            await SetAuthorizationClient();
            var endPoint = $"{NodosApi}/{id}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return null;
            }

            var nodo = await response.Content.ReadAsAsync<ResponseNodoErpAcademico>();
            return _mapper.Map<ResponseNodoErpAcademico, NodoErpAcademicoModel>(nodo);
        }

        public async Task<List<VersionPlanAcademicoModel>> GetVersionesPlanes(VersionPlanListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new VersionPlanAdvancedSearchParameters
            {
                FilterPlan = parameters.FilterIdPlan.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdPlan.Value
                    }
                    : null,
                FilterNro = new RangeFilterParameters<int?>
                {
                    From = parameters.FilterNroFrom,
                    To = parameters.FilterNroTo
                },
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nro",
                OrderDirection = "asc"
            };

            var endPoint = $"{VersionesPlanesApi}/advanced-search";
            var content =
                new ObjectContent<VersionPlanAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<VersionPlanAcademicoModel>();
            }

            var versiones = await response.Content.ReadAsAsync<List<ResponseVersionPlanErpAcademico>>();
            return _mapper.Map<List<ResponseVersionPlanErpAcademico>, List<VersionPlanAcademicoModel>>(versiones);
        }

        public async Task<List<ViaAccesoAcademicoModel>> GetViasAcceso(ViaAccesoListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new ViaAccesoAdvancedSearchParameters
            {
                ProjectViasAccesosGroupedBySuperViaAcceso = parameters.ProjectViasAccesosGroupedBySuperViaAcceso,
                FilterEsVigente = parameters.FilterEsVigente,
                NoPaged = parameters.NoPaged,
                FilterNodo = parameters.FilterIdNodo.HasValue ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdNodo.Value
                } : null,
                ItemsPerPage = parameters.Count,
                PageIndex = parameters.Index,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };
            var endPoint = $"{ViasAccesoApi}/advanced-search";
            var content =
                new ObjectContent<ViaAccesoAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<ViaAccesoAcademicoModel>();
            }

            var viasAcceso = await response.Content.ReadAsAsync<List<ResponseViaAccesoErpAcademico>>();
            return _mapper.Map<List<ResponseViaAccesoErpAcademico>, List<ViaAccesoAcademicoModel>>(viasAcceso);
        }

        public async Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanes(AsignaturaPlanListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new AsignaturaPlanAdvancedSearchParameters
            {
                NoPaged = parameters.NoPaged,
                FilterPlan = parameters.IdPlan.HasValue ? new SimpleItemFilterParameter
                {
                    Id = parameters.IdPlan.Value
                } : null
            };
            var endPoint = $"{AsignaturasPlanesApi}/advanced-search";
            var content =
                new ObjectContent<AsignaturaPlanAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<AsignaturaPlanErpAcademicoModel>();
            }
            var asignaturas = await response.Content.ReadAsAsync<List<ResponseAsignaturaPlanErpAcademico>>();
            return _mapper.Map<List<ResponseAsignaturaPlanErpAcademico>, List<AsignaturaPlanErpAcademicoModel>>(asignaturas);
        }

        #region Es Plan Superado

        public async Task<PlanSuperadoErpAcademicoModel> ItIsPlanSurpassed(int idPlan, EsPlanSuperadoParameters parameters)
        {
            await SetAuthorizationClient();
            var endPoint = $"{PlanesApi}/{idPlan}/es-superado";
            var contentParameters =
                new ObjectContent(typeof(EsPlanSuperadoParameters), parameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, contentParameters);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"[{ticks}] Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                throw new BadRequestException(_localizer[$"ERP Académico [{ticks}]: No se ha podido determinar si el Plan es superado."]);
            }

            var result = await response.Content.ReadAsAsync<ResponsePlanSuperadoErpAcademico>();
            return _mapper.Map<ResponsePlanSuperadoErpAcademico, PlanSuperadoErpAcademicoModel>(result);
        }

        public async Task<ValidateAlumnoMatriculacionErpAcademicoModel> ValidateAlumnoMatriculacion(
            ValidateAlumnoMatriculacionParameters parameters)
        {
            await SetAuthorizationClient();
            var endPoint = $"{ExpedientesApi}/validate-alumno-matriculacion";
            var contentParameters =
                new ObjectContent(typeof(ValidateAlumnoMatriculacionParameters), parameters,
                    new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, contentParameters);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {content}");
                return null;
            }

            var result = await response.Content.ReadAsAsync<ResponseValidateAlumnoMatriculacionErpAcademico>();
            return _mapper
                .Map<ResponseValidateAlumnoMatriculacionErpAcademico, ValidateAlumnoMatriculacionErpAcademicoModel>(
                    result);
        }

        #endregion

        public async Task<List<AsignaturaPlanTitulacionErpAcademico>> GetAsignaturasPlanesParaTitulacion(
            AsignaturasPlanTitulacionParameters parameters)
        {
            await SetAuthorizationClient();
            var endPoint = $"{AsignaturasPlanesApi}/datos-titulacion";
            var content =
                new ObjectContent<AsignaturasPlanTitulacionParameters>(parameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"[{ticks}] Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                throw new BadRequestException(
                    $"[{ticks}] ERP Académico: No se han podido obtener las Asignaturas Planes para los datos de Titulación.");
            }

            var asignaturas = await response.Content.ReadAsAsync<List<ResponseAsignaturaPlanTitulacionErpAcademico>>();
            return _mapper.Map<List<ResponseAsignaturaPlanTitulacionErpAcademico>, List<AsignaturaPlanTitulacionErpAcademico>>(
                asignaturas);
        }

        public async Task<List<int>> GetAsignaturasEspecializacionPlan(int idEspecializacion, int idPlan)
        {
            var result = new List<int>();
            await SetAuthorizationClient();

            var endPoint =
                $"{NodosApi}/asignaturas-especializacion-plan/?IdEspecializacion={idEspecializacion}&IdPlan={idPlan}";
            var response = await _client.GetAsync(endPoint);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<List<int>>();
                return result;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new BadRequestException(
                    $"ERP Académico: No se pueden localizar Asignaturas con el Id Especialización {idEspecializacion} y el IdPlan {idPlan}");
            }

            var contentError = await response.Content.ReadAsStringAsync();
            var ticks = DateTime.UtcNow.Ticks;
            _logger.LogError(
                $"[{ticks}] Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
            return result;
        }

        public async Task<UsuarioModel> GetUserNameByIdSeguridad(string idSeguridad)
        {
            await SetAuthorizationClient();

            var endPoint = $"{SecurityApi}/{idSeguridad}/username";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var ticks = DateTime.UtcNow.Ticks;
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"[{ticks}] Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return null;
            }

            var usuario = await response.Content.ReadAsAsync<string>();
            return new UsuarioModel
            {
                IdCuentaSeguridad = idSeguridad,
                UserName = usuario
            };
        }

        public async Task<List<UniversidadAcademicoModel>> GetUniversidades(UniversidadListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new UniversidadAdvancedSearchParameters
            {
                FilterNombre = parameters.Search,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            var endPoint = $"{UniversidadesApi}/advanced-search";
            var content =
                new ObjectContent<UniversidadAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<UniversidadAcademicoModel>();
            }

            var universidades = await response.Content.ReadAsAsync<List<ResponseUniversidadErpAcademico>>();
            return universidades
                .Select(u => _mapper.Map<ResponseUniversidadErpAcademico, UniversidadAcademicoModel>(u))
                .ToList();
        }

        public async Task<List<EspecializacionIntegrationModel>> GetEspecializaciones(EspecializacionListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new EspecializacionAdvancedSearchParameters
            {
                FilterIdsEspecializaciones = parameters.FilterIdsEspecializaciones,
                PageIndex = parameters.Index,
                OrderColumnName = "codigo",
                ItemsPerPage = parameters.Count,
                OrderDirection = "asc"
            };

            var endPoint = $"{EspecializacionesApi}/advanced-search";
            var content =
                new ObjectContent<EspecializacionAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<EspecializacionIntegrationModel>();
            }

            var versiones = await response.Content.ReadAsAsync<List<ResponseEspecializacionIntegrationModel>>();
            return _mapper.Map<List<ResponseEspecializacionIntegrationModel>, List<EspecializacionIntegrationModel>>(versiones);
        }

        public async Task<EspecializacionAcademicoModel> GetEspecializacion(int especializacionId)
        {
            await SetAuthorizationClient();
            var endPoint = $"{EspecializacionesApi}/{especializacionId}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, especializacionId.ToString());
                _logger.LogError(mensajeError);
                return null;
            }
            var result = await response.Content.ReadAsAsync<ResponseEspecializacionErpAcademico>();
            return _mapper.Map<ResponseEspecializacionErpAcademico, EspecializacionAcademicoModel>(result);
        }

        protected internal virtual async Task SetAuthorizationClient()
        {
            var resultToken = await _securityCrosscuttingService.GetTokenErpAcademico();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultToken);
        }

        public async Task<List<ViaAccesoPlanAcademicoModel>> GetViasAccesosPlanes(ViaAccesoPlanListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new ViaAccesoPlanAdvancedSearchParameters
            {
                NoPaged = parameters.NoPaged,
                FilterExpedientes = parameters.FilterIdExpediente.HasValue ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdExpediente.Value
                } : null,
                FilterViaAcceso = !string.IsNullOrEmpty(parameters.FilterIdViaAcceso) ? new SimpleItemFilterParameter
                {
                    Id = Convert.ToInt32(parameters.FilterIdViaAcceso)
                } : null,
                FilterNodo = parameters.FilterIdNodo.HasValue ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdNodo.Value
                } : null,
                ItemsPerPage = parameters.Count,
                PageIndex = parameters.Index,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };
            var endPoint = $"{ViasAccesosPlanesApi}/advanced-search";
            var content =
                new ObjectContent<ViaAccesoPlanAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<ViaAccesoPlanAcademicoModel>();
            }

            var viasAccesosPlanes = await response.Content.ReadAsAsync<List<ResponseViaAccesoPlanErpAcademico>>();
            return _mapper.Map<List<ResponseViaAccesoPlanErpAcademico>, List<ViaAccesoPlanAcademicoModel>>(viasAccesosPlanes);
        }

        public async Task<DateTime?> GetFirstFechaPrimerMatricula(string idRefIntegracionAlumno, string idRefPlan)
        {
            await SetAuthorizationClient();
            var endPoint =
                $"{MatriculasApi}/first-fecha-pre-matricula?idIntegracionAlumno={idRefIntegracionAlumno}&idPlan={idRefPlan}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return null;
            }

            return await response.Content.ReadAsAsync<DateTime?>();
        }

        public async Task<List<int>> GetIdsPlanesARelacionar(int idPlan, int idEstudio)
        {
            await SetAuthorizationClient();
            var endPoint = $"{PlanesApi}/{idPlan}/estudio/{idEstudio}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<int>();
            }

            var idsPlanes = await response.Content.ReadAsAsync<List<int>>();
            return idsPlanes;
        }

        public async Task<AlumnoAcademicoModel> GetAlumno(int idIntegracionAlumno)
        {
            await SetAuthorizationClient();

            var endPoint = $"{AlumnosApi}/integracion/{idIntegracionAlumno}/datos-basicos";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new AlumnoAcademicoModel();
            }
            var persona = await response.Content.ReadAsAsync<ResponseAlumnoErpAcademico>();
            return _mapper.Map<ResponseAlumnoErpAcademico, AlumnoAcademicoModel>(persona);
        }

        public async Task<AlumnoAcademicoModel> GetAlumnoMatriculasDocumentos(int idIntegracionAlumno)
        {
            await SetAuthorizationClient();

            var endPoint = $"{AlumnosApi}/integracion/{idIntegracionAlumno}/matriculas-documentos";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new AlumnoAcademicoModel();
            }
            var alumno = await response.Content.ReadAsAsync<ResponseAlumnoErpAcademico>();
            return _mapper.Map<ResponseAlumnoErpAcademico, AlumnoAcademicoModel>(alumno);
        }

        public async Task<List<MatriculaErpAcademicoModel>> GetMatriculasAlumno(AlumnoMatriculasParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new
            {
                FilterIdIntegracionAlumno = parameters.IdIntegracionAlumno,
                FilterIdsRefExpedientesAlumnos = parameters.IdsRefExpedientesAlumnos,
                FlatProjection = true,
                ItemsPerPage = parameters.Count,
                PageIndex = parameters.Index,
                OrderDirection = "desc",
                OrderColumnName = "numero",
                HasTotalCreditosAsignaturasMatriculadasActivas = true
            };
            var endPoint = $"{MatriculasApi}/advanced-search";
            var content =
                new ObjectContent<object>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<MatriculaErpAcademicoModel>();
            }

            var contentMatriculas = await response.Content.ReadAsAsync<ResponseContentMatriculaAdvancedSearchErpAcademico>();
            return contentMatriculas.Content.Elements
                .Select(u => _mapper.Map<ResponseMatriculaErpAcademico, MatriculaErpAcademicoModel>(u))
                .ToList();
        }

        public async Task<string> GetFotoAlumnoById(int alumnoId)
        {
            await SetAuthorizationClient();

            var endPoint = $"{AlumnosApi}/{alumnoId}/persona-foto-thumb";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return null;
            }
            return await response.Content.ReadAsAsync<string>();
        }

        public async Task<DocumentoAlumnoModel> GetDocumentosAlumno(string idAlumno)
        {
            await SetAuthorizationClient();

            var endPoint = $"{AlumnosApi}/{idAlumno}/documentos";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new DocumentoAlumnoModel();
            }
            var persona = await response.Content.ReadAsAsync<ResponseDocumentoAlumnoErpAcademico>();
            return _mapper.Map<ResponseDocumentoAlumnoErpAcademico, DocumentoAlumnoModel>(persona);
        }

        public async Task<List<ModoRequerimientoDocumentacionAcademicoModel>> GetModosRequerimientoDocumentacion(
            ModoRequerimientoDocumentacionListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new ModoRequerimientoDocumentacionAdvancedSearchParameters
            {
                FilterNombre = parameters.Search,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc",
                NoPaged = parameters.NoPaged
            };

            var endPoint = $"{ModosRequerimientoDocumentacionApi}/advanced-search";
            var content = new ObjectContent<ModoRequerimientoDocumentacionAdvancedSearchParameters>(
                contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<ModoRequerimientoDocumentacionAcademicoModel>();
            }

            var modosRequerimientoDocumentacion = await response.Content
                .ReadAsAsync<List<ResponseModoRequerimientoDocumentacionErpAcademico>>();
            return modosRequerimientoDocumentacion
                .Select(mrd => _mapper.Map<ResponseModoRequerimientoDocumentacionErpAcademico, ModoRequerimientoDocumentacionAcademicoModel>(mrd))
                .ToList();
        }

        public async Task<List<TipoEstudioAcademicoModel>> GetTiposEstudios(TipoEstudioListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new TipoEstudioAdvancedSearchParameters
            {
                FilterNivelUso = parameters.Search,
                FilterUniversidad = parameters.FilterIdUniversidad != null ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdUniversidad.Value
                } : null,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            const string endPoint = $"{TiposEstudiosApi}/advanced-search";
            var content =
                new ObjectContent<TipoEstudioAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<TipoEstudioAcademicoModel>();
            }

            var tiposEstudios = await response.Content.ReadAsAsync<List<ResponseTipoEstudioErpAcademico>>();
            return tiposEstudios
                .Select(te => _mapper.Map<ResponseTipoEstudioErpAcademico, TipoEstudioAcademicoModel>(te))
                .ToList();
        }

        public async Task<List<EstudioAcademicoModel>> GetEstudios(EstudioListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new EstudioAdvancedSearchParameters
            {
                FilterIdNombre = parameters.Search,
                FilterIds = parameters.FilterIds,
                NoPaged = parameters.NoPaged,
                FilterIdsUniversidades = parameters.FilterIdsUniversidades,
                FilterIdsCentros = parameters.FilterIdsCentros,
                FilterIdsAreasAcademicas = parameters.FilterIdsAreasAcademicas,
                FilterTipo = parameters.FilterIdTipo != null ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdTipo.Value
                } : null,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            const string endPoint = $"{EstudiosApi}/advanced-search";
            var content =
                new ObjectContent<EstudioAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<EstudioAcademicoModel>();
            }

            var estudios = await response.Content.ReadAsAsync<List<ResponseEstudioErpAcademico>>();
            return estudios
                .Select(e => _mapper.Map<ResponseEstudioErpAcademico, EstudioAcademicoModel>(e))
                .ToList();
        }

        public async Task<List<PlanAcademicoModel>> GetPlanes(PlanEstudioListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new PlanEstudioAdvancedSearchParameters
            {
                FilterIdsUniversidades = parameters.FilterIdsUniversidades,
                FilterIdsEstudios = parameters.FilterIdsEstudios,
                FilterCodigoNombre = parameters.Search,
                FilterTipoEstudio = parameters.FilterIdTipoEstudio != null ? new SimpleItemFilterParameter
                {
                    Id = parameters.FilterIdTipoEstudio.Value
                } : null,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            const string endPoint = $"{PlanesApi}/advanced-search";
            var content =
                new ObjectContent<PlanEstudioAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<PlanAcademicoModel>();
            }

            var planesEstudios = await response.Content.ReadAsAsync<List<ResponsePlanErpAcademico>>();
            return planesEstudios
                .Select(pe => _mapper.Map<ResponsePlanErpAcademico, PlanAcademicoModel>(pe))
                .ToList();
        }

        public async Task<List<TipoAsignaturaErpAcademicoModel>> GetTiposAsignaturas(
            TiposAsignaturasListParameters parameters)
        {
            await SetAuthorizationClient();
            const string endPoint = $"{TiposAsignaturasApi}/advanced-search";
            var contentParameters = new TiposAsignaturasAdvancedSearch
            {
                FilterNombre = parameters.Search,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "orden",
                OrderDirection = "asc"
            };
            var content =
                new ObjectContent<TiposAsignaturasAdvancedSearch>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<TipoAsignaturaErpAcademicoModel>();
            }

            var resultResponse = await response.Content.ReadAsAsync<List<ResponseTipoAsignaturaErpAcademico>>();
            return _mapper.Map<List<ResponseTipoAsignaturaErpAcademico>, List<TipoAsignaturaErpAcademicoModel>>(resultResponse);
        }

        public async Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanesParaNivelDeUso(
            AsignaturaPlanListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new AsignaturaPlanAdvancedSearchParameters
            {
                FilterIdsPlanes = parameters.FilterIdsPlanes,
                FilterNivelUso = parameters.Search,
                FilterEstudio = parameters.FilterIdEstudio != null
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdEstudio.Value
                    }
                    : null,
                FilterUniversidad = parameters.FilterIdUniversidad.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdUniversidad.Value
                    }
                    : null,
                FilterTipoEstudio = parameters.FilterIdTipoEstudio.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdTipoEstudio.Value
                    }
                    : null,
                FilterTipoAsignatura = parameters.FilterIdTipoAsignatura.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdTipoAsignatura.Value
                    }
                    : null,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };

            const string endPoint = $"{AsignaturasPlanesApi}/advanced-search";
            var content =
                new ObjectContent<AsignaturaPlanAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<AsignaturaPlanErpAcademicoModel>();
            }

            var asignaturasPlanes = await response.Content.ReadAsAsync<List<ResponseAsignaturaPlanErpAcademico>>();
            return asignaturasPlanes
                .Select(ap => _mapper.Map<ResponseAsignaturaPlanErpAcademico, AsignaturaPlanErpAcademicoModel>(ap))
                .ToList();
        }

        public async Task<ModoRequerimientoDocumentacionAcademicoModel> GetModoRequerimientoDocumentacionById(string idModoRequerimiento)
        {
            await SetAuthorizationClient();

            var endPoint = $"{ModosRequerimientoDocumentacionApi}/{idModoRequerimiento}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new ModoRequerimientoDocumentacionAcademicoModel();
            }
            var modoRequerimiento = await response.Content.ReadAsAsync<ResponseModoRequerimientoDocumentacionErpAcademico>();
            return _mapper.Map<ResponseModoRequerimientoDocumentacionErpAcademico, ModoRequerimientoDocumentacionAcademicoModel>(modoRequerimiento);
        }

        public async Task<List<FileTypeAcademicoModel>> GetFylesTypes()
        {
            await SetAuthorizationClient();

            var endPoint = $"{FilesTypesApi}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    $"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new List<FileTypeAcademicoModel>();
            }
            var filesTypes = await response.Content.ReadAsAsync<List<FileTypeErpAcademico>>();
            return _mapper.Map<List<FileTypeErpAcademico>, List<FileTypeAcademicoModel>>(filesTypes);
        }

        public async Task<UniversidadAcademicoModel> GetUniversidadById(int idUniversidad)
        {
            await SetAuthorizationClient();

            var endPoint = $"{UniversidadesApi}/{idUniversidad}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new UniversidadAcademicoModel();
            }

            var universidad = await response.Content.ReadAsAsync<ResponseUniversidadErpAcademico>();
            return _mapper.Map<ResponseUniversidadErpAcademico, UniversidadAcademicoModel>(universidad);
        }

        public async Task<TipoEstudioAcademicoModel> GetTipoEstudioById(int idTipoEstudio)
        {
            await SetAuthorizationClient();

            var endPoint = $"{TiposEstudiosApi}/{idTipoEstudio}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new TipoEstudioAcademicoModel();
            }

            var tipoEstudio = await response.Content.ReadAsAsync<ResponseTipoEstudioErpAcademico>();
            return _mapper.Map<ResponseTipoEstudioErpAcademico, TipoEstudioAcademicoModel>(tipoEstudio);
        }

        public async Task<EstudioAcademicoModel> GetEstudioById(int idEstudio)
        {
            await SetAuthorizationClient();

            var endPoint = $"{EstudiosApi}/{idEstudio}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new EstudioAcademicoModel();
            }

            var estudio = await response.Content.ReadAsAsync<ResponseEstudioErpAcademico>();
            return _mapper.Map<ResponseEstudioErpAcademico, EstudioAcademicoModel>(estudio);
        }

        public async Task<PlanAcademicoModel> GetPlanEstudioById(int idPlanEstudio)
        {
            await SetAuthorizationClient();

            var endPoint = $"{PlanesApi}/{idPlanEstudio}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new PlanAcademicoModel();
            }

            var planEstudio = await response.Content.ReadAsAsync<ResponsePlanErpAcademico>();
            return _mapper.Map<ResponsePlanErpAcademico, PlanAcademicoModel>(planEstudio);
        }

        public async Task<TipoAsignaturaErpAcademicoModel> GetTipoAsignaturaById(int idTipoAsignatura)
        {
            await SetAuthorizationClient();

            var endPoint = $"{TiposAsignaturasApi}/{idTipoAsignatura}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new TipoAsignaturaErpAcademicoModel();
            }

            var tipoAsignatura = await response.Content.ReadAsAsync<ResponseTipoAsignaturaErpAcademico>();
            return _mapper.Map<ResponseTipoAsignaturaErpAcademico, TipoAsignaturaErpAcademicoModel>(tipoAsignatura);
        }

        public async Task<AsignaturaPlanErpAcademicoModel> GetAsignaturaPlanById(int idAsignatura)
        {
            await SetAuthorizationClient();

            var endPoint = $"{AsignaturasPlanesApi}/{idAsignatura}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var contentError = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error servicio: {endPoint}. StatusCode: {response.StatusCode}. Content: {contentError}");
                return new AsignaturaPlanErpAcademicoModel();
            }

            var asignatura = await response.Content.ReadAsAsync<ResponseAsignaturaPlanErpAcademico>();
            return _mapper.Map<ResponseAsignaturaPlanErpAcademico, AsignaturaPlanErpAcademicoModel>(asignatura);
        }

        public async Task<List<IdiomaAcademicoModel>> GetIdiomas(IdiomaListParameters parameters)
        {
            await SetAuthorizationClient();

            var contentParameters = new IdiomasAdvancedSearchParameters
            {
                FilterNombre = parameters.SearchText,
                NoPaged = true,
                OrderColumnName = "nombre",
                OrderDirection = "asc"
            };
            var content =
                new ObjectContent<IdiomasAdvancedSearchParameters>(contentParameters, new JsonMediaTypeFormatter());

            var endPoint = $"{IdiomasApi}/advanced-search";
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(contentParameters));
                _logger.LogError(mensajeError);
                return new List<IdiomaAcademicoModel>();
            }

            var idiomas = await response.Content.ReadAsAsync<List<ResponseIdiomaErpAcademico>>();
            return _mapper.Map<List<ResponseIdiomaErpAcademico>, List<IdiomaAcademicoModel>>(idiomas);
        }

        public async Task<IdiomaAcademicoModel> GetIdioma(int id)
        {
            await SetAuthorizationClient();
            var endPoint = $"{IdiomasApi}/{id}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, id.ToString());
                _logger.LogError(mensajeError);
                return null;
            }

            var idioma = await response.Content.ReadAsAsync<ResponseIdiomaErpAcademico>();
            return _mapper.Map<ResponseIdiomaErpAcademico, IdiomaAcademicoModel>(idioma);
        }

        public async Task<List<MatriculaAcademicoModel>> GetMatriculasByIdExpediente(int id)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MatriculasApi}/expediente-alumno/{id}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, id.ToString());
                _logger.LogError(mensajeError);
                return new List<MatriculaAcademicoModel>();
            }

            var matriculas = await response.Content.ReadAsAsync<List<ResponseMatriculaAcademicoModel>>();
            return _mapper.Map<List<ResponseMatriculaAcademicoModel>, List<MatriculaAcademicoModel>>(matriculas);
        }

        public async Task<List<RequerimientoPlanAcademicoModel>> GetRequerimientosPlan(
            RequerimientoPlanListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new RequerimientoPlanAdvancedSearchParameters
            {
                FilterPlan = parameters.FilterIdPlan.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdPlan.Value
                    }
                    : null,
                FilterNombre = parameters.SearchText,
                PageIndex = parameters.Index,
                ItemsPerPage = parameters.Count
            };
            var content = new ObjectContent<RequerimientoPlanAdvancedSearchParameters>(
                contentParameters, new JsonMediaTypeFormatter());

            var endPoint = $"{RequerimientosPlanesApi}/advanced-search-for-expedientes";
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(contentParameters));
                _logger.LogError(mensajeError);
                return new List<RequerimientoPlanAcademicoModel>();
            }

            var requerimientos = await response.Content
                .ReadAsAsync<List<ResponseRequerimientoPlanAcademico>>();
            return _mapper.Map<List<ResponseRequerimientoPlanAcademico>,
                List<RequerimientoPlanAcademicoModel>>(requerimientos);
        }

        public async Task<List<TrayectoPlanModel>> GetTrayectosRequerimientosPlan(
            RequerimientoPlanListParameters parameters)
        {
            var requerimientos = await GetRequerimientosPlan(parameters);
            var trayectosPlanes = requerimientos.SelectMany(r => r.TrayectosPlanes).ToList();
            return trayectosPlanes;
        }

        public async Task<RequerimientoPlanAcademicoModel> GetRequerimientoByIdPlanAndIdRequerimiento(
            int idPlan, int idRequerimiento)
        {
            await SetAuthorizationClient();
            var endPoint = $"{PlanesApi}/{idPlan}/requerimientos/{idRequerimiento}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"Plan: {idPlan}, Requerimiento: {idRequerimiento}");
                _logger.LogError(mensajeError);
                return new RequerimientoPlanAcademicoModel();
            }

            var requerimientos = await response.Content.ReadAsAsync<ResponseRequerimientoPlanAcademico>();
            return _mapper.Map<ResponseRequerimientoPlanAcademico, RequerimientoPlanAcademicoModel>(requerimientos);
        }

        public async Task<MatriculaAcademicoModel> GetMatriculaByIdIntegracionMatricula(string idIntegracionMatricula)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MatriculasApi}/integracion/{idIntegracionMatricula}/datos-basicos";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idIntegracionMatricula);
                _logger.LogError(mensajeError);
                return null;
            }

            var matricula = await response.Content.ReadAsAsync<ResponseMatriculaAcademicoModel>();
            return _mapper.Map<ResponseMatriculaAcademicoModel, MatriculaAcademicoModel>(matricula);
        }

        private async Task<string> SetErrorMessage(HttpResponseMessage response, string body)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var mensajeError = string.Format(
            "[{0}] " +
            "Error con el Servicio de ERP Académico : {1} , Method: {2} , Headers: {3} , Body: {4} , Mensaje del error: {5} , StatusCode: {6}",
            ticks,
            response.RequestMessage.RequestUri,
            response.RequestMessage.Method,
            response.RequestMessage.Headers,
            body,
            await response.Content.ReadAsStringAsync(),
            response.StatusCode);
            return mensajeError;
        }

        public async Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatricula(int idMatricula)
        {
            await SetAuthorizationClient();
            var endPoint = $"{MatriculasApi}/{idMatricula}/asignaturas-matriculadas";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idMatricula.ToString());
                _logger.LogError(mensajeError);
                return new List<AsignaturaMatriculadaModel>();
            }
            var asignaturasMatricula = await response.Content.ReadAsAsync<ResponseResultListErpAcademico<ResponseAsignaturaMatricula>>();
            return _mapper.Map<List<ResponseAsignaturaMatricula>, List<AsignaturaMatriculadaModel>>(asignaturasMatricula.Elements);
        }

        public async Task<List<AsignaturaOfertadaMigracionModel>> GetAsignaturasOfertadasByExpediente(List<string> idsExpediente)
        {
            await SetAuthorizationClient();
            var endPoint = $"{PublicAsignaturasOfertadasApi}/migracion";
            var content = new StringContent(JsonSerializer.Serialize(idsExpediente), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, $"id expediente: {string.Join(",", idsExpediente)}");
                _logger.LogError(mensajeError);
                return null;
            }
            var asignaturasMatricula = await response.Content.ReadAsAsync<ResponseResultListErpAcademico<AsignaturaOfertadaMigracionModel>>();
            return asignaturasMatricula.Elements;
        }

        public async Task<CausaBajaMatriculaModel> GetCausaBajaMatricula(int idCausa)
        {
            await SetAuthorizationClient();
            var endPoint = $"{CausasBajasMatriculaApi}/{idCausa}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idCausa.ToString());
                _logger.LogError(mensajeError);
                return null;
            }
            var causaMatricula = await response.Content.ReadAsAsync<ResponseCausaBajaMatricula>();
            return _mapper.Map<ResponseCausaBajaMatricula, CausaBajaMatriculaModel>(causaMatricula);
        }

        public async Task<TipoBajaMatriculaModel> GetTipoBajaMatricula(int idTipo)
        {
            await SetAuthorizationClient();

            var endPoint = $"{TipoBajasMatriculaApi}/{idTipo}";
            var response = await _client.GetAsync(endPoint);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, idTipo.ToString());
                _logger.LogError(mensajeError);
                return null;
            }
            var tipoBajaMatricula = await response.Content.ReadAsAsync<ResponseTipoBajaMatriculaErpAcademico>();
            return _mapper.Map<ResponseTipoBajaMatriculaErpAcademico, TipoBajaMatriculaModel>(tipoBajaMatricula);
        }

        public async Task<GrafoAcademicoModel> GetGrafoDePlan(NodosListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new
            {
                FilterPlan = parameters.FilterIdPlan.HasValue
                    ? new SimpleItemFilterParameter
                    {
                        Id = parameters.FilterIdPlan.Value
                    }
                    : null
            };
            var content = new ObjectContent<object>(contentParameters, new JsonMediaTypeFormatter());
            var endPoint = "Nodo/GetGrafoDePlan";
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(contentParameters));
                _logger.LogError(mensajeError);
                return null;
            }
            var grafo = await response.Content.ReadAsAsync<ResponseGrafoDataErpAcademico>();
            var grafoDto = _mapper.Map<ResponseGrafoDataErpAcademico, GrafoDataAcademicoModel>(grafo);
            return grafoDto?.Data;
        }

        public async Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatriculadasParaNotaFinal(
            int idRefCurso, List<string> idRefsAlumno)
        {
            await SetAuthorizationClient();

            var parameters = new AsignaturaMatriculadaCursoAlumnosParameters
            {
                FilterIdRefCurso = idRefCurso,
                FilterIdRefsAlumno = idRefsAlumno
            };

            const string endPoint = $"{AsignaturasMatriculadasApi}/curso/alumnos";
            var content =
                new ObjectContent<AsignaturaMatriculadaCursoAlumnosParameters>(parameters,
                    new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(parameters));
                _logger.LogError(mensajeError);
                return null;
            }

            var asignaturasPlanes = await response.Content.ReadAsAsync<List<ResponseAsignaturaMatricula>>();
            return asignaturasPlanes
                .Select(ap => _mapper.Map<ResponseAsignaturaMatricula, AsignaturaMatriculadaModel>(ap))
                .ToList();
        }

        public async Task<List<PlanEstudioErpAcademicoModel>> GetPlanesEstudios(PlanesListParameters parameters)
        {
            await SetAuthorizationClient();
            var contentParameters = new PlanesComboListParameters
            {
                FilterDisplayName = parameters.FilterDisplayName,
                PageIndex = parameters.Index,
                Pagina = parameters.Index,
                OrderColumnName = "codigo",                
                OrderDirection = "asc"
            };

            var endPoint = $"{PlanesApi}/combo-list";
            var content = new ObjectContent<PlanesComboListParameters>(contentParameters, 
                new JsonMediaTypeFormatter());
            var response = await _client.PostAsync(endPoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var mensajeError = await SetErrorMessage(response, JsonSerializer.Serialize(parameters));
                _logger.LogError(mensajeError);
                return null;
            }

            var planesEstudios = await response.Content.ReadAsAsync<List<ResponsePlanEstudioErpAcademico>>();
            return _mapper.Map<List<ResponsePlanEstudioErpAcademico>, 
                List<PlanEstudioErpAcademicoModel>>(planesEstudios);
        }
    }
}